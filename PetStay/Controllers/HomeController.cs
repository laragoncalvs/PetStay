using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStay.Data;
using PetStay.Models;
using System.Diagnostics;

namespace PetStay.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var hotels = await _context.Hotels
                .Where(hotel => hotel.Ativo && hotel.Aprovado && hotel.StatusAprovacao == StatusAprovacaoHotel.Aprovado)
                .AsNoTracking()
                .ToListAsync();

            // Carregar pets do usuário autenticado
            var usuarioAtual = await _userManager.GetUserAsync(User);
            var petsDoTutor = new List<Pet>();
            if (usuarioAtual != null)
            {
                petsDoTutor = await _context.Pets
                    .AsNoTracking()
                    .Where(p => p.TutorId == usuarioAtual.Id)
                    .ToListAsync();
            }

            // Passar dados à view usando ViewBag
            ViewBag.PetsDoTutor = petsDoTutor;

            return View(hotels);
        }

        public async Task<IActionResult> Details(int id, string? cidade, string? estado, DateTime? dataEntrada, DateTime? dataSaida, int? petId)
        {
            var hotel = await _context.Hotels
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id && h.Ativo && h.Aprovado && h.StatusAprovacao == StatusAprovacaoHotel.Aprovado);

            if (hotel is null)
            {
                return NotFound();
            }

            // Carregar pets do usuário autenticado
            var petsDoTutor = new List<Pet>();
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                petsDoTutor = await _context.Pets
                    .Where(p => p.TutorId == user.Id)
                    .ToListAsync();
            }

            var viewModel = new HotelDetailsViewModel
            {
                Hotel = hotel,
                Cidade = cidade,
                Estado = estado,
                DataEntrada = dataEntrada,
                DataSaida = dataSaida,
                PetId = petId,
                PetsDoTutor = petsDoTutor
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
