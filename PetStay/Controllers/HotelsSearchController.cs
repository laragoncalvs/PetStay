using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStay.Data;
using PetStay.Models;

namespace PetStay.Controllers
{
    public class HotelsSearchController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HotelsSearchController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            string? cidade, string? estado, DateTime? dataEntrada, DateTime? dataSaida, int? petId,
            string? raca, int? tamanho, int? faixaEtaria, decimal? valorMinimo, decimal? valorMaximo, 
            string? cuidados, string sortBy = "avaliacao")
        {
            var viewModel = new HotelSearchViewModel
            {
                Cidade = cidade,
                Estado = estado,
                DataEntrada = dataEntrada,
                DataSaida = dataSaida,
                PetId = petId,
                Raca = raca,
                Tamanho = tamanho,
                FaixaEtaria = faixaEtaria,
                ValorMinimo = valorMinimo,
                ValorMaximo = valorMaximo,
                Cuidados = cuidados,
                SortBy = sortBy
            };

            // Carregar pets do tutor autenticado se existir
            var usuarioAtual = await _userManager.GetUserAsync(User);
            if (usuarioAtual != null)
            {
                viewModel.PetsDoTutor = await _context.Pets
                    .AsNoTracking()
                    .Where(p => p.TutorId == usuarioAtual.Id)
                    .ToListAsync();

                // Se um pet foi selecionado, carregá-lo
                if (petId.HasValue)
                {
                    viewModel.PetSelecionado = viewModel.PetsDoTutor.FirstOrDefault(p => p.Id == petId.Value);

                    // Se não encontrou o pet, tentar carregar do banco
                    if (viewModel.PetSelecionado == null)
                    {
                        viewModel.PetSelecionado = await _context.Pets
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.Id == petId.Value && p.TutorId == usuarioAtual.Id);
                    }
                }
            }

            // Se houver filtros, executar busca
            if (!string.IsNullOrWhiteSpace(cidade) || dataEntrada.HasValue)
            {
                viewModel.FoiBuscado = true;

                // Validar datas
                if (dataEntrada.HasValue && dataSaida.HasValue && dataEntrada >= dataSaida)
                {
                    viewModel.MensagemErro = "A data de saída deve ser após a data de entrada.";
                    return View(viewModel);
                }

                // Buscar hotéis aprovados e ativos
                var hotelsQuery = _context.Hotels
                    .AsNoTracking()
                    .Where(h => h.Ativo && h.Aprovado && h.StatusAprovacao == StatusAprovacaoHotel.Aprovado);

                // Filtrar por cidade
                if (!string.IsNullOrWhiteSpace(cidade))
                {
                    hotelsQuery = hotelsQuery.Where(h => h.Cidade.Contains(cidade));
                }

                // Filtrar por estado
                if (!string.IsNullOrWhiteSpace(estado))
                {
                    hotelsQuery = hotelsQuery.Where(h => h.Estado == estado);
                }

                var hoteis = await hotelsQuery
                    .OrderBy(h => h.Cidade)
                    .ToListAsync();

                viewModel.Resultados = new List<HotelSearchResultViewModel>();

                foreach (var hotel in hoteis)
                {
                    // Calcular vagas disponíveis no período
                    int vagasOcupadas = 0;

                    if (dataEntrada.HasValue && dataSaida.HasValue)
                    {
                        vagasOcupadas = await _context.Reservations
                            .AsNoTracking()
                            .Where(r => r.HotelId == hotel.Id
                                && r.Status != StatusReserva.Cancelada
                                && r.DataEntrada < dataSaida.Value
                                && r.DataSaida > dataEntrada.Value)
                            .CountAsync();
                    }

                    int vagasDisponiveis = hotel.Capacidade - vagasOcupadas;

                    // Apenas mosrar hotéis com vagas
                    if (vagasDisponiveis > 0)
                    {
                        int diasReserva = dataSaida.HasValue && dataEntrada.HasValue
                            ? (int)(dataSaida.Value - dataEntrada.Value).TotalDays
                            : 0;

                        decimal valorTotal = diasReserva > 0 ? hotel.Diaria * diasReserva : 0;

                        var resultado = new HotelSearchResultViewModel
                        {
                            Id = hotel.Id,
                            Nome = hotel.Nome,
                            Endereco = hotel.Endereco,
                            Cidade = hotel.Cidade,
                            Estado = hotel.Estado,
                            Telefone = hotel.Telefone,
                            Email = hotel.Email,
                            Descricao = hotel.Descricao,
                            Avaliacao = hotel.Avaliacao,
                            ImagemUrl = hotel.ImagemUrl,
                            Diaria = hotel.Diaria,
                            Capacidade = hotel.Capacidade,
                            VagasDisponiveis = vagasDisponiveis,
                            HorarioAbertura = hotel.HorarioAbertura,
                            HorarioFechamento = hotel.HorarioFechamento,
                            PortesAceitos = hotel.PortesAceitos,
                            FaixasEtariasAceitas = hotel.FaixasEtariasAceitas,
                            ValorTotalEstimado = valorTotal,
                            DiasReserva = diasReserva
                        };

                        viewModel.Resultados.Add(resultado);
                    }
                }

                // Aplicar filtragem adicional
                // Verificar compatibilidade com o pet selecionado
                if (viewModel.PetSelecionado != null)
                {
                    viewModel.Resultados = viewModel.Resultados
                        .Where(h => VerificaCompatibilidadePet(
                            new Hotel 
                            { 
                                PortesAceitos = h.PortesAceitos, 
                                FaixasEtariasAceitas = h.FaixasEtariasAceitas 
                            }, 
                            viewModel.PetSelecionado, 
                            raca, 
                            tamanho, 
                            faixaEtaria, 
                            cuidados))
                        .ToList();
                }

                // Aplicar filtro de valor
                if (valorMinimo.HasValue)
                {
                    viewModel.Resultados = viewModel.Resultados
                        .Where(h => h.Diaria >= valorMinimo.Value)
                        .ToList();
                }

                if (valorMaximo.HasValue)
                {
                    viewModel.Resultados = viewModel.Resultados
                        .Where(h => h.Diaria <= valorMaximo.Value)
                        .ToList();
                }

                // Aplicar ordenação
                viewModel.Resultados = sortBy?.ToLower() switch
                {
                    "valor" => viewModel.Resultados.OrderBy(h => h.Diaria).ToList(),
                    "distancia" => viewModel.Resultados.OrderBy(h => h.Cidade).ToList(), // Ordenar por cidade como aproximação de distância
                    _ => viewModel.Resultados.OrderByDescending(h => h.Avaliacao).ToList() // Padrão: avaliação
                };

                if (viewModel.Resultados.Count == 0)
                {
                    viewModel.MensagemErro = "Nenhum hotel encontrado com as características solicitadas.";
                }
            }

            return View(viewModel);
        }

        private bool VerificaCompatibilidadePet(Hotel hotel, Pet? pet, string? racaFiltro, int? tamanhoFiltro, int? faixaEtariaFiltro, string? cuidadosFiltro)
        {
            // Se não há pet selecionado, tudo é compatível
            if (pet == null)
            {
                return true;
            }

            // Verificar tamanho (PortesAceitos contém Pequeno, Médio, Grande)
            if (!string.IsNullOrWhiteSpace(hotel.PortesAceitos))
            {
                string tamanhoNome = pet.Tamanho switch
                {
                    TamanhoPet.Pequeno => "Pequeno",
                    TamanhoPet.Medio => "Médio",
                    TamanhoPet.Grande => "Grande",
                    _ => ""
                };

                if (!string.IsNullOrEmpty(tamanhoNome) && !hotel.PortesAceitos.Contains(tamanhoNome))
                {
                    return false;
                }
            }

            // Verificar faixa etária (FaixasEtariasAceitas contém Filhote, Adulto, Idoso)
            if (!string.IsNullOrWhiteSpace(hotel.FaixasEtariasAceitas))
            {
                string faixaNome = pet.FaixaEtaria switch
                {
                    FaixaEtariaPet.Filhote => "Filhote",
                    FaixaEtariaPet.Adulto => "Adulto",
                    FaixaEtariaPet.Idoso => "Idoso",
                    _ => ""
                };

                if (!string.IsNullOrEmpty(faixaNome) && !hotel.FaixasEtariasAceitas.Contains(faixaNome))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
