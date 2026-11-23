using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStay.Data;
using PetStay.Models;
using System.Linq;

namespace PetStay.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("reservations")]
        public async Task<IActionResult> Reservations(DateTime? dataInicio, DateTime? dataFim)
        {
            // Verificar se o usuário é gestor
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.Perfil != PerfilUsuario.Gestor)
            {
                return Unauthorized("Apenas gestores podem acessar relatórios.");
            }

            // Definir datas padrão (últimos 30 dias)
            var fim = dataFim?.Date ?? DateTime.Now.Date;
            var inicio = dataInicio?.Date ?? fim.AddDays(-30);

            // Buscar TODOS os hotéis do gestor
            var hoteisDoGestor = await _context.Hotels
                .AsNoTracking()
                .Where(h => h.GestorId == (user.GestorDonoId ?? user.Id) && h.Ativo && h.Aprovado)
                .ToListAsync();

            if (!hoteisDoGestor.Any())
            {
                return NotFound("Este gestor não possui hotéis cadastrados.");
            }

            var idsHoteis = hoteisDoGestor.Select(h => h.Id).ToList();

            // Buscar TODAS as reservas de TODOS os hotéis no período
            var reservas = await _context.Reservations
                .AsNoTracking()
                .Where(r => idsHoteis.Contains(r.HotelId) &&
                       ((r.DataEntrada.Date >= inicio && r.DataEntrada.Date <= fim) ||
                        (r.DataSaida.Date >= inicio && r.DataSaida.Date <= fim) ||
                        (r.DataEntrada.Date < inicio && r.DataSaida.Date > fim)))
                .Include(r => r.Pet)
                .Include(r => r.Tutor)
                .Include(r => r.Hotel)
                .ToListAsync();

            // Calcular capacidade total de todos os hotéis
            int capacidadeTotal = hoteisDoGestor.Sum(h => h.Capacidade);

            // Calcular métricas
            var model = new ReservationReportViewModel
            {
                DataInicio = inicio,
                DataFim = fim,
                HotelId = 0, // Não é mais por hotel
                NomeHotel = "Todos os Hotéis",
                CapacidadeHotel = capacidadeTotal,

                // Por status
                TotalPendentes = reservas.Count(r => r.Status == StatusReserva.Pendente),
                TotalConfirmadas = reservas.Count(r => r.Status == StatusReserva.Confirmada),
                TotalEmProgresso = reservas.Count(r => r.Status == StatusReserva.EmProgresso),
                TotalFinalizadas = reservas.Count(r => r.Status == StatusReserva.Finalizada),
                TotalCanceladas = reservas.Count(r => r.Status == StatusReserva.Cancelada),

                // Totalizadores
                TotalFaturado = reservas
                    .Where(r => r.Status == StatusReserva.Finalizada || r.Status == StatusReserva.EmProgresso)
                    .Sum(r => r.ValorTotal),
                ReservasConfirmadas = reservas.Count(r => r.Status == StatusReserva.Confirmada || 
                                                           r.Status == StatusReserva.EmProgresso)
            };

            // Calcular faturamento diário
            model.FaturaimentoPorDia = CalcularFaturamentoPorDia(reservas, inicio, fim);

            // Calcular ocupação diária
            model.OcupacaoPorDia = CalcularOcupacaoPorDia(reservas, inicio, fim, capacidadeTotal);

            // Calcular taxa de ocupação média
            if (model.OcupacaoPorDia.Any())
            {
                model.OcupacaoMedia = Math.Round(model.OcupacaoPorDia.Average(o => o.Ocupacao), 2);
                model.TaxaOcupacao = Math.Round(model.OcupacaoPorDia.Average(o => o.Ocupacao), 2);
            }

            // Montar lista de detalhes (com nome do hotel)
            model.Reservas = reservas
                .OrderByDescending(r => r.DataEntrada)
                .Select(r => new ReservationDetalheDto
                {
                    Id = r.Id,
                    PetNome = r.Pet?.Nome ?? "Desconhecido",
                    TutorNome = r.Tutor?.Nome ?? "Desconhecido",
                    HotelNome = r.Hotel?.Nome ?? "Desconhecido",
                    DataEntrada = r.DataEntrada,
                    DataSaida = r.DataSaida,
                    Status = r.Status,
                    ValorTotal = r.ValorTotal,
                    DiariasReservadas = (int)(r.DataSaida.Date - r.DataEntrada.Date).TotalDays
                })
                .ToList();

            return View(model);
        }

        private List<DadoFaturamentoDiario> CalcularFaturamentoPorDia(List<Reservation> reservas, DateTime inicio, DateTime fim)
        {
            var resultado = new List<DadoFaturamentoDiario>();

            for (var data = inicio; data <= fim; data = data.AddDays(1))
            {
                var reavasDoDia = reservas
                    .Where(r => (r.DataEntrada.Date <= data && r.DataSaida.Date > data) &&
                               r.Status != StatusReserva.Cancelada)
                    .ToList();

                decimal valor = 0;
                if (reavasDoDia.Any())
                {
                    // Distribuir valor total pela quantidade de dias
                    valor = reavasDoDia.Sum(r =>
                    {
                        int diasReserva = (int)(r.DataSaida.Date - r.DataEntrada.Date).TotalDays;
                        return diasReserva > 0 ? r.ValorTotal / diasReserva : 0;
                    });
                }

                resultado.Add(new DadoFaturamentoDiario
                {
                    Data = data,
                    Valor = Math.Round(valor, 2),
                    QuantidadeReservas = reavasDoDia.Count
                });
            }

            return resultado;
        }

        private List<DadoOcupacaoDia> CalcularOcupacaoPorDia(List<Reservation> reservas, DateTime inicio, DateTime fim, int capacidade)
        {
            var resultado = new List<DadoOcupacaoDia>();

            if (capacidade <= 0) capacidade = 1; // Evitar divisão por zero

            for (var data = inicio; data <= fim; data = data.AddDays(1))
            {
                // Contar reservas que estão ativas neste dia (excluindo canceladas)
                var hóspedesOcupando = reservas
                    .Count(r => r.DataEntrada.Date <= data &&
                               r.DataSaida.Date > data &&
                               r.Status != StatusReserva.Cancelada);

                var ocupacao = (decimal)hóspedesOcupando / capacidade * 100;

                resultado.Add(new DadoOcupacaoDia
                {
                    Data = data,
                    Ocupacao = Math.Min(100, Math.Round(ocupacao, 2)),
                    HospedesOcupando = hóspedesOcupando
                });
            }

            return resultado;
        }
    }
}
