using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStay.Data;
using PetStay.Models;
using PetStay.Models.DTOs;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PetStay.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Payment/Index (sem reservationId - pega do Session)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Buscar carrinho da session
            var cartJson = HttpContext.Session.GetString("ReservationCart");
            if (string.IsNullOrEmpty(cartJson))
                return RedirectToAction("Index", "HotelsSearch");

            var cart = System.Text.Json.JsonSerializer.Deserialize<ReservationCartDto>(cartJson);
            if (cart == null)
                return RedirectToAction("Index", "HotelsSearch");

            // Buscar dados para exibição
            var hotel = await _context.Hotels.FindAsync(cart.HotelId);
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == cart.PetId && p.TutorId == user.Id);

            if (hotel == null || pet == null)
                return RedirectToAction("Index", "HotelsSearch");

            var viewModel = new PaymentViewModel
            {
                ValorTotal = cart.ValorTotal,
                HotelNome = hotel.Nome,
                PetNome = pet.Nome,
                DataEntrada = cart.DataEntrada,
                DataSaida = cart.DataSaida
            };

            return View(viewModel);
        }

        // POST: Payment/ConfirmAndPay (AJAX com JSON body)
        [HttpPost("confirm-and-pay")]
        public async Task<IActionResult> ConfirmAndPay([FromBody] PaymentProcessingDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Buscar carrinho da session
            var cartJson = HttpContext.Session.GetString("ReservationCart");
            if (string.IsNullOrEmpty(cartJson))
                return BadRequest(new { message = "Carrinho expirado. Reinicie o processo." });

            var cart = System.Text.Json.JsonSerializer.Deserialize<ReservationCartDto>(cartJson);
            if (cart == null)
                return BadRequest(new { message = "Erro ao processar carrinho." });

            // Validar método de pagamento
            if (string.IsNullOrEmpty(dto.MetodoPagamento))
                return BadRequest(new { message = "Selecione um método de pagamento." });

            // Mock de processamento de pagamento
            var codigoTransacao = await ProcessarPagamentoMock(new PaymentViewModel 
            { 
                MetodoPagamento = dto.MetodoPagamento,
                NumeroCartao = dto.NumeroCartao,
                ValidadeCartao = dto.ValidadeCartao,
                CvvCartao = dto.CvvCartao,
                NomeCartao = dto.NomeCartao
            });

            if (string.IsNullOrEmpty(codigoTransacao))
                return BadRequest(new { message = "Falha ao processar o pagamento." });

            // AGORA criar a reserva após pagamento confirmado
            var reservation = new Reservation
            {
                PetId = cart.PetId,
                HotelId = cart.HotelId,
                TutorId = user.Id,
                DataEntrada = cart.DataEntrada,
                DataSaida = cart.DataSaida,
                HorarioEntrada = cart.HorarioEntrada,
                HorarioSaida = cart.HorarioSaida,
                ObservacoesTutor = cart.ObservacoesTutor,
                ValorDiariaBase = cart.ValorDiariaBase,
                ValorAdicionalIdade = cart.ValorAdicionalIdade,
                ValorTotal = cart.ValorTotal,
                Status = StatusReserva.Pendente,
                SolicitadaEm = DateTime.UtcNow,
                CodigoTransacao = codigoTransacao,
                CriadaEm = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Limpar session
            HttpContext.Session.Remove("ReservationCart");

            return Ok(new
            {
                message = "Pagamento processado com sucesso!",
                reservationId = reservation.Id,
                codigoTransacao = codigoTransacao,
                status = reservation.Status,
                redirectUrl = Url.Action("Details", "Reservation", new { id = reservation.Id })
            });
        }

        // Métodos auxiliares
        private async Task<string?> ProcessarPagamentoMock(PaymentViewModel model)
        {
            // Simular processamento de pagamento
            // Em produção, isso seria uma integração real com um gateway de pagamento

            await Task.Delay(1000); // Simular delay de processamento

            // Validar dados mínimos
            if (model.MetodoPagamento == "pix")
            {
                // PIX é automático - aceita sempre
                return GeraCodigoTransacao();
            }
            else if (model.MetodoPagamento == "cartao_credito" || model.MetodoPagamento == "cartao_debito")
            {
                // Validar dados do cartão
                if (string.IsNullOrEmpty(model.NumeroCartao) || 
                    string.IsNullOrEmpty(model.ValidadeCartao) || 
                    string.IsNullOrEmpty(model.CvvCartao))
                    return null;

                // Simular validação básica
                // Em produção, seria chamada à API do gateway (Stripe, MercadoPago, etc)
                // Aqui aceitamos como válido
                return GeraCodigoTransacao();
            }

            return null;
        }

        private string GeraCodigoTransacao()
        {
            // Formato: TXN-YYYYMMDD-RANDOMHEX
            var timestamp = DateTime.Now.ToString("yyyyMMdd");
            var random = Guid.NewGuid().ToString().Substring(0, 12).ToUpper();
            return $"TXN-{timestamp}-{random}";
        }
    }
}
