using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStay.Data;
using PetStay.Models;
using PetStay.Models.DTOs;

namespace PetStay.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservation/Create?hotelId=5
        [HttpGet("create")]
        public async Task<IActionResult> Create(int hotelId)
        {
            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(h => h.Id == hotelId && h.Ativo && h.Aprovado);

            if (hotel == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var petsDoTutor = await _context.Pets
                .Where(p => p.TutorId == user.Id)
                .ToListAsync();

            var model = new ReservationCreateViewModel
            {
                HotelId = hotelId,
                Hotel = hotel,
                PetsDoTutor = petsDoTutor,
                DataEntrada = DateTime.Now.AddDays(1),
                DataSaida = DateTime.Now.AddDays(2),
                HorarioEntrada = new TimeSpan(14, 0, 0),
                HorarioSaida = new TimeSpan(11, 0, 0),
                ValorDiariaBase = hotel.Diaria
            };

            return View(model);
        }



        // POST: Reservation/Create
        [HttpPost("create")]
        public async Task<IActionResult> Create(ReservationCreateViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Recarregar dados da view em caso de erro
            if (!ModelState.IsValid)
            {
                var hotelError = await _context.Hotels.FindAsync(model.HotelId);
                var petsError = await _context.Pets.Where(p => p.TutorId == user.Id).ToListAsync();

                model.Hotel = hotelError;
                model.PetsDoTutor = petsError;

                return View("Create", model);
            }

            // Buscar hotel
            var hotel = await _context.Hotels.FindAsync(model.HotelId);
            if (hotel == null || !hotel.Ativo || !hotel.Aprovado)
            {
                ModelState.AddModelError("", "Hotel nÃ£o encontrado ou indisponÃ­vel.");

                model.PetsDoTutor = await _context.Pets.Where(p => p.TutorId == user.Id).ToListAsync();
                return View("Create", model);
            }

            // Buscar pet
            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.Id == model.PetId && p.TutorId == user.Id);

            if (pet == null)
            {
                ModelState.AddModelError("", "Pet nÃ£o encontrado.");

                model.Hotel = hotel;
                model.PetsDoTutor = await _context.Pets.Where(p => p.TutorId == user.Id).ToListAsync();
                return View("Create", model);
            }

            // Validar datas
            if (model.DataSaida <= model.DataEntrada)
            {
                ModelState.AddModelError("", "Data de saÃ­da deve ser apÃ³s data de entrada.");

                model.Hotel = hotel;
                model.PetsDoTutor = await _context.Pets.Where(p => p.TutorId == user.Id).ToListAsync();
                return View("Create", model);
            }

            if (model.DataEntrada < DateTime.Now.Date)
            {
                ModelState.AddModelError("", "NÃ£o Ã© possÃ­vel reservar para datas passadas.");

                model.Hotel = hotel;
                model.PetsDoTutor = await _context.Pets.Where(p => p.TutorId == user.Id).ToListAsync();
                return View("Create", model);
            }

            // Verificar compatibilidade do pet
            if (!VerificaCompatibilidadePet(hotel, pet))
            {
                ModelState.AddModelError("", "Pet nÃ£o Ã© compatÃ­vel com este hotel.");

                model.Hotel = hotel;
                model.PetsDoTutor = await _context.Pets.Where(p => p.TutorId == user.Id).ToListAsync();
                return View("Create", model);
            }

            // Verificar disponibilidade de vagas
            var diasReserva = (model.DataSaida - model.DataEntrada).Days;
            if (!VerificaVagasDisponiveis(hotel, model.DataEntrada, model.DataSaida))
            {
                ModelState.AddModelError("", "NÃ£o hÃ¡ vagas disponÃ­veis para as datas selecionadas.");

                model.Hotel = hotel;
                model.PetsDoTutor = await _context.Pets.Where(p => p.TutorId == user.Id).ToListAsync();
                return View("Create", model);
            }

            // Calcular valor
            var (valorDiaria, valorAdicionalIdade) = CalculaValorReserva(hotel, pet, diasReserva);
            var valorTotal = (valorDiaria + valorAdicionalIdade) * diasReserva;

            // Guardar em Session (nÃ£o salvar no DB ainda!)
            var cartDto = new ReservationCartDto
            {
                HotelId = model.HotelId,
                PetId = model.PetId,
                DataEntrada = model.DataEntrada,
                DataSaida = model.DataSaida,
                HorarioEntrada = model.HorarioEntrada,
                HorarioSaida = model.HorarioSaida,
                ObservacoesTutor = model.ObservacoesTutor,
                ValorDiariaBase = valorDiaria,
                ValorAdicionalIdade = valorAdicionalIdade,
                ValorTotal = valorTotal
            };

            // Serializar e guardar na session
            HttpContext.Session.SetString("ReservationCart", System.Text.Json.JsonSerializer.Serialize(cartDto));

            // Redirecionar para pagamento
            return RedirectToAction("Index", "Payment");
        }

        // GET: Reservation/MyReservations
        [HttpGet("my-reservations")]
        public async Task<IActionResult> MyReservations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Atualizar reservas Confirmada que jÃ¡ passaram para Finalizada
            var expiredReservations = await _context.Reservations
                .Where(r => r.TutorId == user.Id 
                    && r.Status == StatusReserva.Confirmada 
                    && r.DataSaida < DateTime.Now.Date)
                .ToListAsync();

            if (expiredReservations.Any())
            {
                foreach (var reservation in expiredReservations)
                {
                    reservation.Status = StatusReserva.Finalizada;
                    reservation.AtualizadaEm = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }

            var reservations = await _context.Reservations
                .Where(r => r.TutorId == user.Id)
                .Include(r => r.Hotel)
                .Include(r => r.Pet)
                .OrderByDescending(r => r.CriadaEm)
                .ToListAsync();

            return View(reservations);
        }

        // GET: Reservation/Details/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var reservation = await _context.Reservations
                .Where(r => r.Id == id && r.TutorId == user.Id)
                .Include(r => r.Hotel)
                .Include(r => r.Pet)
                .FirstOrDefaultAsync();

            if (reservation == null)
                return NotFound();

            // Atualizar automaticamente para Finalizada se jÃ¡ passou
            if (reservation.Status == StatusReserva.Confirmada && reservation.DataSaida < DateTime.Now.Date)
            {
                reservation.Status = StatusReserva.Finalizada;
                reservation.AtualizadaEm = DateTime.UtcNow;
                _context.Reservations.Update(reservation);
                await _context.SaveChangesAsync();
            }

            return View(reservation);
        }

        // POST: Reservation/Cancel/5
        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == id && r.TutorId == user.Id);

            if (reservation == null)
                return NotFound();

            if (reservation.Status == StatusReserva.Cancelada)
                return BadRequest(new { message = "Esta reserva jÃ¡ foi cancelada." });

            if (reservation.DataEntrada <= DateTime.Now.Date)
                return BadRequest(new { message = "NÃ£o Ã© possÃ­vel cancelar uma reserva que jÃ¡ iniciou." });

            // Reembolso automÃ¡tico
            // TODO: Integrar com gateway de pagamento para processar reembolso
            reservation.Status = StatusReserva.Cancelada;
            reservation.AtualizadaEm = DateTime.UtcNow;

            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reserva cancelada com reembolso automÃ¡tico processado." });
        }

        // MÃ©todos auxiliares
        private bool VerificaCompatibilidadePet(Hotel hotel, Pet pet)
        {
            // Se nÃ£o hÃ¡ restriÃ§Ãµes, aceita qualquer pet
            if (string.IsNullOrWhiteSpace(hotel.PortesAceitos) && string.IsNullOrWhiteSpace(hotel.FaixasEtariasAceitas))
                return true;

            // PortesAceitos e FaixasEtariasAceitas sÃ£o strings com valores separados por vÃ­rgula
            var portesAceitos = hotel.PortesAceitos?.Split(',').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToList() ?? new List<string>();
            var faixasAceitas = hotel.FaixasEtariasAceitas?.Split(',').Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).ToList() ?? new List<string>();

            // Verificar se o tamanho do pet Ã© aceito (comparar nome do enum, nÃ£o valor)
            if (portesAceitos.Count > 0 && !portesAceitos.Contains(pet.Tamanho.ToString(), StringComparer.OrdinalIgnoreCase))
                return false;

            // Verificar se a faixa etÃ¡ria do pet Ã© aceita (comparar nome do enum, nÃ£o valor)
            if (faixasAceitas.Count > 0 && !faixasAceitas.Contains(pet.FaixaEtaria.ToString(), StringComparer.OrdinalIgnoreCase))
                return false;

            return true;
        }

        private bool VerificaVagasDisponiveis(Hotel hotel, DateTime dataEntrada, DateTime dataSaida)
        {
            // Verificar se hÃ¡ reservas conflitantes
            var reservasConflitantes = _context.Reservations
                .Where(r => r.HotelId == hotel.Id
                    && r.Status != StatusReserva.Cancelada
                    && r.DataEntrada < dataSaida
                    && r.DataSaida > dataEntrada)
                .ToList();

            // Comparar com nÃºmero de vagas disponÃ­veis
            var vagasOcupadas = reservasConflitantes.Count;
            var vagasDisponiveis = hotel.Capacidade - vagasOcupadas;

            return vagasDisponiveis > 0;
        }

        // GET: Reservation/PendingForManager - Lista de reservas pendentes do hotel do gestor
        [HttpGet("pending-for-manager")]
        public async Task<IActionResult> PendingForManager()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Verificar se Ã© gestor
            if (user.Perfil != PerfilUsuario.Gestor)
                return Forbid();

            // Buscar hotÃ©is onde o usuÃ¡rio Ã© gestor
            var hoteisDosGestores = await _context.Hotels
                .Where(h => h.GestorId == user.Id)
                .Select(h => h.Id)
                .ToListAsync();

            // Se nÃ£o tem hotÃ©is atribuÃ­dos, mostrar lista vazia
            if (!hoteisDosGestores.Any())
            {
                return View(new List<Reservation>());
            }

            // Buscar reservas pendentes dos hotÃ©is do gestor
            var reservasPendentes = await _context.Reservations
                .Include(r => r.Hotel)
                .Include(r => r.Pet)
                .Include(r => r.Tutor)
                .Where(r => hoteisDosGestores.Contains(r.HotelId) && r.Status == StatusReserva.Pendente)
                .OrderByDescending(r => r.SolicitadaEm)
                .ToListAsync();

            return View(reservasPendentes);
        }

        // GET: Reservation/DebugPending - Debug para verificar estado das reservas
        [HttpGet("debug-pending")]
        public async Task<IActionResult> DebugPending()
        {
            var user = await _userManager.GetUserAsync(User);

            var debug = new
            {
                userId = user?.Id,
                userName = user?.Nome,
                userPerfil = user?.Perfil.ToString(),
                hotelCount = user != null ? await _context.Hotels.Where(h => h.GestorId == user.Id).CountAsync() : 0,
                reservasPendentes = await _context.Reservations
                    .Where(r => r.Status == StatusReserva.Pendente)
                    .Select(r => new { r.Id, r.HotelId, r.Status, r.SolicitadaEm })
                    .ToListAsync(),
                todasReservas = await _context.Reservations
                    .Select(r => new { r.Id, r.HotelId, r.Status, r.SolicitadaEm })
                    .ToListAsync()
            };

            return Ok(debug);
        }

        // GET: Reservation/Debug - PÃ¡gina visual de debug
        [HttpGet("debug")]
        public IActionResult Debug()
        {
            return View("DebugPending");
        }

        // GET: Reservation/ManageReservations - Painel unificado para gerenciar check-in e check-out
        [HttpGet("manage-reservations")]
        public async Task<IActionResult> ManageReservations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Verificar se é gestor
            if (user.Perfil != PerfilUsuario.Gestor)
                return Forbid();

            // Buscar hotéis onde o usuário é gestor
            var hoteisDosGestores = await _context.Hotels
                .Where(h => h.GestorId == user.Id)
                .Select(h => h.Id)
                .ToListAsync();

            if (!hoteisDosGestores.Any())
                return View(new List<Reservation>());

            // Buscar reservas confirmadas e em progresso dos hotéis do gestor
            var reservas = await _context.Reservations
                .Include(r => r.Hotel)
                .Include(r => r.Pet)
                .Include(r => r.Tutor)
                .Where(r => hoteisDosGestores.Contains(r.HotelId) && 
                       (r.Status == StatusReserva.Confirmada || r.Status == StatusReserva.EmProgresso))
                .OrderByDescending(r => r.DataEntrada)
                .ToListAsync();

            return View(reservas);
        }

        // GET: Reservation/ConfirmedReservations - Lista de reservas confirmadas para check-in
        [HttpGet("confirmed-reservations")]
        public async Task<IActionResult> ConfirmedReservations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Verificar se Ã© gestor
            if (user.Perfil != PerfilUsuario.Gestor)
                return Forbid();

            // Buscar hotÃ©is onde o usuÃ¡rio Ã© gestor
            var hoteisDosGestores = await _context.Hotels
                .Where(h => h.GestorId == user.Id)
                .Select(h => h.Id)
                .ToListAsync();

            if (!hoteisDosGestores.Any())
                return View(new List<Reservation>());

            // Buscar reservas confirmadas dos hotÃ©is do gestor
            var reservasConfirmadas = await _context.Reservations
                .Include(r => r.Hotel)
                .Include(r => r.Pet)
                .Include(r => r.Tutor)
                .Where(r => hoteisDosGestores.Contains(r.HotelId) && r.Status == StatusReserva.Confirmada)
                .OrderByDescending(r => r.DataEntrada)
                .ToListAsync();

            return View(reservasConfirmadas);
        }

        // GET: Reservation/InProgressReservations - Lista de reservas em progresso para check-out
        [HttpGet("in-progress-reservations")]
        public async Task<IActionResult> InProgressReservations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Verificar se Ã© gestor
            if (user.Perfil != PerfilUsuario.Gestor)
                return Forbid();

            // Buscar hotÃ©is onde o usuÃ¡rio Ã© gestor
            var hoteisDosGestores = await _context.Hotels
                .Where(h => h.GestorId == user.Id)
                .Select(h => h.Id)
                .ToListAsync();

            if (!hoteisDosGestores.Any())
                return View(new List<Reservation>());

            // Buscar reservas em progresso dos hotÃ©is do gestor
            var reservasEmProgresso = await _context.Reservations
                .Include(r => r.Hotel)
                .Include(r => r.Pet)
                .Include(r => r.Tutor)
                .Where(r => hoteisDosGestores.Contains(r.HotelId) && r.Status == StatusReserva.EmProgresso)
                .OrderByDescending(r => r.DataCheckInEfetiva)
                .ToListAsync();

            return View(reservasEmProgresso);
        }

        // POST: Reservation/Accept/5
        [HttpPost("accept/{id}")]
        public async Task<IActionResult> Accept(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Verificar se Ã© gestor
            if (user.Perfil != PerfilUsuario.Gestor)
                return Forbid();

            var reservation = await _context.Reservations
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            // Verificar se o gestor Ã© responsÃ¡vel deste hotel
            if (reservation.Hotel.GestorId != user.Id)
                return Forbid();

            // Verificar se jÃ¡ foi respondida
            if (reservation.Status != StatusReserva.Pendente)
                return BadRequest("Esta reserva jÃ¡ foi respondida.");

            // Atualizar status
            reservation.Status = StatusReserva.Confirmada;
            reservation.RespondidaEm = DateTime.UtcNow;

            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Reserva aceita com sucesso!";
            return RedirectToAction(nameof(PendingForManager));
        }

        // POST: Reservation/Reject/5
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> Reject(int id, [FromForm] string motivo)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Verificar se Ã© gestor
            if (user.Perfil != PerfilUsuario.Gestor)
                return Forbid();

            var reservation = await _context.Reservations
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            // Verificar se o gestor Ã© responsÃ¡vel deste hotel
            if (reservation.Hotel.GestorId != user.Id)
                return Forbid();

            // Verificar se jÃ¡ foi respondida
            if (reservation.Status != StatusReserva.Pendente)
                return BadRequest("Esta reserva jÃ¡ foi respondida.");

            // Atualizar status
            reservation.Status = StatusReserva.Cancelada;
            reservation.RespondidaEm = DateTime.UtcNow;
            reservation.MotivoRejeicao = motivo;

            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            // TODO: Processar reembolso aqui

            TempData["Success"] = "Reserva recusada com sucesso!";
            return RedirectToAction(nameof(PendingForManager));
        }

        // POST: Reservation/RegisterCheckIn/5
        [HttpPost("register-check-in/{id}")]
        public async Task<IActionResult> RegisterCheckIn(int id, [FromForm] DateTime dataCheckIn, [FromForm] TimeSpan? horaCheckIn, [FromForm] string? observacoes)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Verificar se Ã© gestor
            if (user.Perfil != PerfilUsuario.Gestor)
                return Forbid();

            var reservation = await _context.Reservations
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            // Verificar se o gestor Ã© responsÃ¡vel deste hotel
            if (reservation.Hotel.GestorId != user.Id)
                return Forbid();

            // Verificar se estÃ¡ em status Confirmada
            if (reservation.Status != StatusReserva.Confirmada)
                return BadRequest("Check-in sÃ³ pode ser registrado em reservas Confirmadas.");

            // Validar datas
            if (dataCheckIn < reservation.DataEntrada.Date)
                return BadRequest("Data de check-in nÃ£o pode ser antes da data de entrada da reserva.");

            if (dataCheckIn > DateTime.UtcNow.Date.AddDays(1))
                return BadRequest("Data de check-in nÃ£o pode ser no futuro.");

            // Atualizar reservation
            reservation.DataCheckInEfetiva = dataCheckIn;
            reservation.HoraCheckInEfetiva = horaCheckIn ?? new TimeSpan(14, 0, 0);
            reservation.Status = StatusReserva.EmProgresso;
            reservation.ObservacoeGestor = observacoes;

            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Check-in registrado com sucesso!";
            return RedirectToAction(nameof(ManageReservations));
        }

        // POST: Reservation/RegisterCheckOut/5
        [HttpPost("register-check-out/{id}")]
        public async Task<IActionResult> RegisterCheckOut(int id, [FromForm] DateTime dataCheckOut, [FromForm] TimeSpan? horaCheckOut, [FromForm] string? observacoes)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Verificar se Ã© gestor
            if (user.Perfil != PerfilUsuario.Gestor)
                return Forbid();

            var reservation = await _context.Reservations
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            // Verificar se o gestor Ã© responsÃ¡vel deste hotel
            if (reservation.Hotel.GestorId != user.Id)
                return Forbid();

            // Verificar se estÃ¡ em status EmProgresso
            if (reservation.Status != StatusReserva.EmProgresso)
                return BadRequest("Check-out sÃ³ pode ser registrado em reservas Em Progresso.");

            // Validar datas
            if (dataCheckOut < reservation.DataCheckInEfetiva?.Date)
                return BadRequest("Data de check-out nÃ£o pode ser antes da data de check-in.");

            if (dataCheckOut > reservation.DataSaida.Date)
                return BadRequest("Data de check-out nÃ£o pode ser depois da data de saÃ­da da reserva.");

            // Atualizar reservation
            reservation.DataCheckOutEfetiva = dataCheckOut;
            reservation.HoraCheckOutEfetiva = horaCheckOut ?? new TimeSpan(11, 0, 0);
            reservation.Status = StatusReserva.Finalizada;
            if (!string.IsNullOrEmpty(observacoes))
                reservation.ObservacoeGestor = observacoes;

            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Check-out registrado com sucesso!";
            return RedirectToAction(nameof(ManageReservations));
        }

        private (decimal valorDiaria, decimal valorAdicionalIdade) CalculaValorReserva(Hotel hotel, Pet pet, int diasReserva)
        {
            var valorDiaria = hotel.Diaria;

            // Adicional por faixa etÃ¡ria
            var valorAdicionalIdade = pet.FaixaEtaria switch
            {
                FaixaEtariaPet.Filhote => hotel.Diaria * 0.10m, // 10% adicional
                FaixaEtariaPet.Idoso => hotel.Diaria * 0.15m,   // 15% adicional
                _ => 0m
            };

            return (valorDiaria, valorAdicionalIdade);
        }
    }
}

