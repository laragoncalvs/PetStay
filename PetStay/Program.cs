using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetStay.Data;
using PetStay.Models;
using PetStay.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(); // Adicionar HttpClient para requisições externas
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<AppDbContext>();

// Registrar o serviço de auto-cancelamento de reservas
builder.Services.AddHostedService<AutoCancelReservationService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

await SeedAdminAsync(app.Services, builder.Configuration);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

static async Task SeedAdminAsync(IServiceProvider services, IConfiguration configuration)
{
    var email = configuration["AdminSeed:Email"] ?? "admin@petstay.com";
    var password = configuration["AdminSeed:Password"];

    if (string.IsNullOrWhiteSpace(password))
    {
        return;
    }

    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var adminRole = nameof(PerfilUsuario.Administrador);
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        var roleResult = await roleManager.CreateAsync(new IdentityRole<int>(adminRole));
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException("Não foi possível criar o papel Administrador.");
        }
    }

    var admin = await userManager.FindByEmailAsync(email);
    if (admin is null)
    {
        admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            Nome = "Administrador",
            Perfil = PerfilUsuario.Administrador,
            TermosAceitos = true,
            TermosAceitosEm = DateTime.UtcNow,
            Ativo = true
        };

        var userResult = await userManager.CreateAsync(admin, password);
        if (!userResult.Succeeded)
        {
            var errors = string.Join("; ", userResult.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Não foi possível criar o administrador: {errors}");
        }
    }

    if (!admin.Ativo)
    {
        admin.Ativo = true;
        var updateResult = await userManager.UpdateAsync(admin);
        if (!updateResult.Succeeded)
        {
            throw new InvalidOperationException("Não foi possível ativar o administrador.");
        }
    }
    else if (admin.Perfil != PerfilUsuario.Administrador)
    {
        admin.Perfil = PerfilUsuario.Administrador;
        var updateResult = await userManager.UpdateAsync(admin);
        if (!updateResult.Succeeded)
        {
            throw new InvalidOperationException("Não foi possível atualizar o perfil do administrador.");
        }
    }

    if (!await userManager.IsInRoleAsync(admin, adminRole))
    {
        var roleResult = await userManager.AddToRoleAsync(admin, adminRole);
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException("Não foi possível atribuir o papel Administrador ao usuário.");
        }
    }
}
