using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetStay.Data;
using PetStay.Models;

namespace PetStay.Controllers
{
    [Authorize(Roles = nameof(PerfilUsuario.Gestor))]
    public class HotelsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HotelsController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new HotelCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotelCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var gestor = await _userManager.GetUserAsync(User);
            if (gestor is null)
            {
                return Challenge();
            }

            var hotel = new Hotel
            {
                Nome = model.Nome,
                Endereco = model.Endereco,
                Cidade = model.Cidade,
                Estado = model.Estado,
                Telefone = model.Telefone,
                Email = model.Email,
                Descricao = model.Descricao,
                Capacidade = model.Capacidade,
                Diaria = model.Diaria,
                ImagemUrl = model.ImagemUrl,
                HorarioAbertura = model.HorarioAbertura,
                HorarioFechamento = model.HorarioFechamento,
                PortesAceitos = string.Join(",", model.PortesAceitos.Select(porte => (int)porte)),
                FaixasEtariasAceitas = string.Join(",", model.FaixasEtariasAceitas.Select(faixa => (int)faixa)),
                GestorId = gestor.Id,
                Aprovado = false,
                Ativo = false,
                StatusAprovacao = StatusAprovacaoHotel.Pendente
            };

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            TempData["HotelSuccess"] = "Hotel cadastrado e enviado para aprovação.";
            return RedirectToAction(nameof(Create));
        }
    }
}
