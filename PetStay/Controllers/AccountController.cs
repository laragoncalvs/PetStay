using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PetStay.Models;

namespace PetStay.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                return View(model);
            }
            if (!user.Ativo)
            {
                var senhaCorreta = await _userManager.CheckPasswordAsync(user, model.Senha);
                ModelState.AddModelError(string.Empty, senhaCorreta
                    ? "Sua conta está inativa. Entre em contato com o administrador."
                    : "E-mail ou senha inválidos.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Senha,
                model.LembrarMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Sua conta está temporariamente bloqueada. Tente novamente mais tarde.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Challenge();
            }

            return View(CreateProfileViewModel(user));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile([Bind(Prefix = "DadosPessoais")] EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return await ProfileWithModelAsync(new ProfileViewModel
                {
                    DadosPessoais = model
                });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Challenge();
            }

            user.Nome = model.Nome;
            user.PhoneNumber = model.Telefone;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                AddIdentityErrors(result);
                return await ProfileWithModelAsync(new ProfileViewModel
                {
                    DadosPessoais = model
                });
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["ProfileSuccess"] = "Seus dados foram atualizados com sucesso.";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([Bind(Prefix = "AlteracaoSenha")] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return await ProfileWithModelAsync(new ProfileViewModel
                {
                    AlteracaoSenha = model
                });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Challenge();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.SenhaAtual, model.NovaSenha);
            if (!result.Succeeded)
            {
                AddIdentityErrors(result);
                return await ProfileWithModelAsync(new ProfileViewModel
                {
                    AlteracaoSenha = model
                });
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["PasswordSuccess"] = "Sua senha foi alterada com sucesso.";
            return RedirectToAction(nameof(Profile));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            
            if (model.Perfil == PerfilUsuario.Administrador)
            {
                ModelState.AddModelError(nameof(model.Perfil), "Não é permitido cadastrar administradores.");
                return View(model);
            }

            var criador = await _userManager.GetUserAsync(User);
            int? gestorDonoId = null;

            if (model.Perfil == PerfilUsuario.Gestor)
            {
                if (criador == null)
                {
                    ModelState.AddModelError(nameof(model.Perfil), "É necessário estar autenticado para cadastrar gestores.");
                    return View(model);
                }

                if (User.IsInRole(nameof(PerfilUsuario.Administrador)))
                {
                    gestorDonoId = null;
                }
                else if (User.IsInRole(nameof(PerfilUsuario.Gestor)) && criador.GestorDonoId == null)
                {
                    gestorDonoId = criador.Id;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Perfil), "Você não tem permissão para cadastrar gestores.");
                    return View(model);
                }
            }


            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Telefone,
                Nome = model.Nome,
                Perfil = model.Perfil!.Value,
                GestorDonoId = gestorDonoId,
                TermosAceitos = model.AceitouTermos,
                TermosAceitosEm = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Senha);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            var roleName = user.Perfil.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }
            }

            
            var resultadoPerfil = await _userManager.AddToRoleAsync(user, roleName);
            if (!resultadoPerfil.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                foreach (var error in resultadoPerfil.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            if (criador != null)
            {
                if (User.IsInRole(nameof(PerfilUsuario.Administrador)))
                    return RedirectToAction("Index", "AdminUsers");

                return RedirectToAction("Index", "Home");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");

        }

        private ProfileViewModel CreateProfileViewModel(ApplicationUser user)
        {
            return new ProfileViewModel
            {
                DadosPessoais = new EditProfileViewModel
                {
                    Nome = user.Nome,
                    Telefone = user.PhoneNumber ?? string.Empty
                }
            };
        }

        private async Task<IActionResult> ProfileWithModelAsync(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Challenge();
            }

            if (string.IsNullOrWhiteSpace(model.DadosPessoais.Nome))
            {
                model.DadosPessoais.Nome = user.Nome;
            }

            if (string.IsNullOrWhiteSpace(model.DadosPessoais.Telefone))
            {
                model.DadosPessoais.Telefone = user.PhoneNumber ?? string.Empty;
            }

            return View("Profile", model);
        }

        private void AddIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
