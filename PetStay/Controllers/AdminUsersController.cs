using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStay.Models;

namespace PetStay.Controllers
{
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminUsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? busca)
        {
            var usuarioAtual = await _userManager.GetUserAsync(User);
            if (usuarioAtual is null)
            {
                return Challenge();
            }

            var query = _userManager.Users.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(busca))
            {
                busca = busca.Trim();
                query = query.Where(user =>
                    user.Nome.Contains(busca) ||
                    user.Email!.Contains(busca));
            }

            var usuarios = await query
                .OrderBy(user => user.Nome)
                .Select(user => new AdminUserItemViewModel
                {
                    Id = user.Id,
                    Nome = user.Nome,
                    Email = user.Email ?? string.Empty,
                    Perfil = user.Perfil,
                    Ativo = user.Ativo,
                    EhUsuarioAtual = user.Id == usuarioAtual.Id
                })
                .ToListAsync();

            return View(new AdminUsersViewModel
            {
                Busca = busca,
                Usuarios = usuarios
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Activate(int id)
        {
            return SetStatusAsync(id, true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Deactivate(int id)
        {
            return SetStatusAsync(id, false);
        }

        private async Task<IActionResult> SetStatusAsync(int id, bool ativo)
        {
            var usuarioAtual = await _userManager.GetUserAsync(User);
            if (usuarioAtual is null)
            {
                return Challenge();
            }

            if (usuarioAtual.Id == id && !ativo)
            {
                TempData["AdminUserError"] = "Você não pode inativar a própria conta.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario is null)
            {
                return NotFound();
            }

            usuario.Ativo = ativo;
            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                TempData["AdminUserError"] = "Não foi possível atualizar o status do usuário.";
                return RedirectToAction(nameof(Index));
            }

            TempData["AdminUserSuccess"] = ativo
                ? $"O usuário {usuario.Nome} foi ativado."
                : $"O usuário {usuario.Nome} foi inativado.";

            return RedirectToAction(nameof(Index));
        }
    }
}
