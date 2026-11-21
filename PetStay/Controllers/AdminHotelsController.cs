using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStay.Data;
using PetStay.Models;

namespace PetStay.Controllers
{
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public class AdminHotelsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminHotelsController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var hotels = await _context.Hotels
                .AsNoTracking()
                .Include(hotel => hotel.Gestor)
                .Where(hotel =>
                    hotel.StatusAprovacao == StatusAprovacaoHotel.Pendente ||
                    (!hotel.Aprovado && hotel.StatusAprovacao != StatusAprovacaoHotel.Reprovado))
                .OrderBy(hotel => hotel.Id)
                .ToListAsync();

            return View(hotels);
        }

        [HttpGet]
        public async Task<IActionResult> Review(int id)
        {
            var hotel = await GetPendingHotelAsync(id);
            if (hotel is null)
            {
                return NotFound();
            }

            return View(CreateReviewViewModel(hotel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int id, HotelReviewViewModel model)
        {
            var hotel = await GetPendingHotelAsync(id);
            if (hotel is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                PopulateReviewViewModel(model, hotel);
                return View(model);
            }

            var administrador = await _userManager.GetUserAsync(User);
            if (administrador is null)
            {
                return Challenge();
            }

            hotel.StatusAprovacao = model.Decisao!.Value == DecisaoHotel.Aprovado
                ? StatusAprovacaoHotel.Aprovado
                : StatusAprovacaoHotel.Reprovado;
            hotel.Aprovado = model.Decisao.Value == DecisaoHotel.Aprovado;
            hotel.Ativo = hotel.Aprovado;
            hotel.JustificativaAprovacao = model.Justificativa.Trim();
            hotel.AprovadoEm = DateTime.UtcNow;
            hotel.AdministradorId = administrador.Id;

            await _context.SaveChangesAsync();

            TempData["AdminHotelSuccess"] = hotel.Aprovado
                ? $"O hotel {hotel.Nome} foi aprovado."
                : $"O hotel {hotel.Nome} foi reprovado.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<Hotel?> GetPendingHotelAsync(int id)
        {
            return await _context.Hotels
                .Include(hotel => hotel.Gestor)
                .FirstOrDefaultAsync(hotel =>
                    hotel.Id == id &&
                    (hotel.StatusAprovacao == StatusAprovacaoHotel.Pendente ||
                     (!hotel.Aprovado && hotel.StatusAprovacao != StatusAprovacaoHotel.Reprovado)));
        }

        private static HotelReviewViewModel CreateReviewViewModel(Hotel hotel)
        {
            var model = new HotelReviewViewModel
            {
                HotelId = hotel.Id,
                NomeHotel = hotel.Nome,
                Cidade = hotel.Cidade,
                GestorNome = hotel.Gestor?.Nome
            };

            return model;
        }

        private static void PopulateReviewViewModel(HotelReviewViewModel model, Hotel hotel)
        {
            model.HotelId = hotel.Id;
            model.NomeHotel = hotel.Nome;
            model.Cidade = hotel.Cidade;
            model.GestorNome = hotel.Gestor?.Nome;
        }
    }
}
