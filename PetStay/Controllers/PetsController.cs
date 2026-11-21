using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStay.Data;
using PetStay.Models;

namespace PetStay.Controllers
{
    [Authorize(Roles = nameof(PerfilUsuario.Tutor))]
    public class PetsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PetsController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tutor = await _userManager.GetUserAsync(User);
            if (tutor is null)
            {
                return Challenge();
            }

            var pets = await _context.Pets
                .AsNoTracking()
                .Where(pet => pet.TutorId == tutor.Id)
                .OrderBy(pet => pet.Nome)
                .ToListAsync();

            return View(pets);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new PetCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetCreateViewModel model)
        {
            if (!model.DeclaracaoVacinacao)
            {
                ModelState.AddModelError(
                    nameof(PetCreateViewModel.DeclaracaoVacinacao),
                    "É necessário declarar que a vacinação está em dia.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var tutor = await _userManager.GetUserAsync(User);
            if (tutor is null)
            {
                return Challenge();
            }

            var pet = new Pet
            {
                Nome = model.Nome,
                Especie = model.Especie,
                Raca = model.Raca,
                Tamanho = model.Tamanho!.Value,
                FaixaEtaria = model.FaixaEtaria!.Value,
                CuidadosEspecificos = model.CuidadosEspecificos,
                DeclaracaoVacinacao = model.DeclaracaoVacinacao,
                TutorId = tutor.Id
            };

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            TempData["PetSuccess"] = $"O pet {pet.Nome} foi cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tutor = await _userManager.GetUserAsync(User);
            if (tutor is null)
            {
                return Challenge();
            }

            var pet = await _context.Pets
                .AsNoTracking()
                .FirstOrDefaultAsync(pet => pet.Id == id && pet.TutorId == tutor.Id);

            if (pet is null)
            {
                return NotFound();
            }

            return View(ToViewModel(pet));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PetCreateViewModel model)
        {
            if (!model.DeclaracaoVacinacao)
            {
                ModelState.AddModelError(
                    nameof(PetCreateViewModel.DeclaracaoVacinacao),
                    "É necessário declarar que a vacinação está em dia.");
            }

            var tutor = await _userManager.GetUserAsync(User);
            if (tutor is null)
            {
                return Challenge();
            }

            var pet = await _context.Pets
                .FirstOrDefaultAsync(pet => pet.Id == id && pet.TutorId == tutor.Id);

            if (pet is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            pet.Nome = model.Nome;
            pet.Especie = model.Especie;
            pet.Raca = model.Raca;
            pet.Tamanho = model.Tamanho!.Value;
            pet.FaixaEtaria = model.FaixaEtaria!.Value;
            pet.CuidadosEspecificos = model.CuidadosEspecificos;
            pet.DeclaracaoVacinacao = model.DeclaracaoVacinacao;

            await _context.SaveChangesAsync();
            TempData["PetSuccess"] = $"Os dados de {pet.Nome} foram atualizados com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tutor = await _userManager.GetUserAsync(User);
            if (tutor is null)
            {
                return Challenge();
            }

            var pet = await _context.Pets
                .FirstOrDefaultAsync(pet => pet.Id == id && pet.TutorId == tutor.Id);

            if (pet is null)
            {
                return NotFound();
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            TempData["PetSuccess"] = $"O pet {pet.Nome} foi excluído com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private static PetCreateViewModel ToViewModel(Pet pet)
        {
            return new PetCreateViewModel
            {
                Nome = pet.Nome,
                Especie = pet.Especie,
                Raca = pet.Raca,
                Tamanho = pet.Tamanho,
                FaixaEtaria = pet.FaixaEtaria,
                CuidadosEspecificos = pet.CuidadosEspecificos,
                DeclaracaoVacinacao = pet.DeclaracaoVacinacao
            };
        }
    }
}
