# 🔍 FAQ e Troubleshooting - PetStay

## ❓ Perguntas Frequentes

### Como faço para resetar a senha do admin?

```csharp
// Execute este código no Program.cs ou no SeedAdminAsync
var admin = await userManager.FindByEmailAsync("admin@petsstay.com");
if (admin != null)
{
	var token = await userManager.GeneratePasswordResetTokenAsync(admin);
	await userManager.ResetPasswordAsync(admin, token, "NewPassword@123");
}
```

### Como altero a connection string para meu banco local?

Edite `appsettings.json`:
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=.\\SQLEXPRESS;Database=PetStayDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Como vejo os logs de erro?

Verifique o arquivo de log ou console output:
```bash
dotnet run
```

Configure mais verboso em `appsettings.json`:
```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Debug",
	  "Microsoft.AspNetCore": "Information"
	}
  }
}
```

### Como faço deploy em produção?

```bash
# Publicar Release
dotnet publish -c Release -o ./publish

# Azure App Service
az webapp deployment source config-zip \
  --resource-group PetStayRG \
  --name petstay-app \
  --src ./publish.zip
```

### Como integro um email provider?

Configure em `appsettings.json`:
```json
{
  "EmailSettings": {
	"Provider": "smtp.gmail.com",
	"Port": 587,
	"FromEmail": "seu-email@gmail.com",
	"Password": "app-password",
	"EnableSSL": true
  }
}
```

Depois use:
```csharp
await _emailService.SendEmailAsync(user.Email, subject, body);
```

---

## 🐛 Resolvendo Problemas Comuns

### Erro: "ConnectionString argument is invalid"

**Causa:** String de conexão malformada

**Solução:**
```bash
# Verificar string de conexão
cat appsettings.json # Linux/Mac
type appsettings.json # Windows

# Testar conexão com SQL Server
sqlcmd -S localhost -U sa -P sua-senha
```

---

### Erro: "DbContext configuration error"

**Causa:** Banco não foi criado ou migrations pendentes

**Solução:**
```bash
# Aplicar migrations
dotnet ef database update

# Ou resetar banco
dotnet ef database drop -f
dotnet ef database update
```

---

### Erro: "Port 5000 already in use"

**Causa:** Outra aplicação usando a porta

**Solução:**
```bash
# Usar porta diferente
dotnet run --urls "http://localhost:3000"

# Ou matar processo na porta (Windows)
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Ou (Linux/Mac)
lsof -i :5000
kill -9 <PID>
```

---

### Erro: "NuGet packages not found"

**Causa:** Packages não foram restaurados

**Solução:**
```bash
# Limpar cache
dotnet nuget locals all --clear

# Restaurar packages
dotnet restore

# Se persistir, deletar node_modules/packages.lock.json
rm -rf .packages/
dotnet restore
```

---

### Erro: "Migration conflict"

**Causa:** Múltiplas migrations criadas no mesmo contexto

**Solução:**
```bash
# Ver migrations pendentes
dotnet ef migrations list

# Remover última migration (se ainda não aplicada)
dotnet ef migrations remove

# Recriar corretamente
dotnet ef migrations add NomeMigracao
dotnet ef database update
```

---

### Erro: "Authorization failed"

**Causa:** Usuário não tem permissão

**Solução:**
1. Verificar se usuário está logado
2. Verificar se usuário tem role correto
3. Verificar claims e policies em Program.cs

```csharp
// Verificar role do usuário
var user = await userManager.GetUserAsync(User);
var roles = await userManager.GetRolesAsync(user);
```

---

### Erro: "Model state invalid"

**Causa:** Dados enviados não validam

**Solução:**
```csharp
if (!ModelState.IsValid)
{
	// Ver erros específicos
	foreach (var modelState in ModelState.Values)
	{
		foreach (var error in modelState.Errors)
		{
			Console.WriteLine(error.ErrorMessage);
		}
	}
}
```

---

### Erro: "Invalid column name 'XYZ'"

**Causa:** Coluna não existe no banco (migration não aplicada)

**Solução:**
```bash
# Verificar last migration
dotnet ef migrations has-pending-migrations

# Aplicar todas pendentes
dotnet ef database update
```

---

### Erro: "CORS policy blocked"

**Causa:** Requisição cross-origin bloqueada

**Solução:**
Configure CORS em `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", b =>
		b.AllowAnyOrigin()
		 .AllowAnyMethod()
		 .AllowAnyHeader());
});

app.UseCors("AllowAll");
```

---

### Erro: "SSL Certificate validation failed"

**Causa:** Ambiente de desenvolvimento com HTTPS

**Solução:**
```bash
# Desabilitar validação SSL (apenas dev!)
export ASPNETCORE_ENVIRONMENT=Development

# Ou aceitar certificado auto-assinado
dotnet ef database update --startup-project .

# HTTPS válido com:
dotnet dev-certs https --trust
```

---

## 📊 Performance

### A aplicação está lenta

```csharp
// 1. Adicionar índices
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	modelBuilder.Entity<Reservation>()
		.HasIndex(r => r.HotelId);
}

// 2. Usar projection (SelectAsync)
var reservations = await _context.Reservations
	.Select(r => new ReservationDto 
	{ 
		Id = r.Id,
		Name = r.Pet.Name
	})
	.ToListAsync();

// 3. Adicionar caching
var cached = await _cache.GetOrCreateAsync("reservations", async entry =>
{
	entry.SlidingExpiration = TimeSpan.FromMinutes(10);
	return await _context.Reservations.ToListAsync();
});
```

---

## 🔐 Segurança

### Dados sensíveis no código?

**NUNCA commite:**
- Senhas
- Connection strings
- API keys
- Tokens

**Use:**
```bash
dotnet user-secrets init
dotnet user-secrets set "Admin:Password" "SuperSecret!23"
```

### Validar entrada do usuário

```csharp
[Required]
[StringLength(100)]
[RegularExpression(@"^[a-zA-Z0-9]*$")]
public string Username { get; set; }
```

---

## 📈 Monitoramento

### Logs em produção

```csharp
// Usar Serilog
builder.Host.UseSerilog((context, config) =>
	config.WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day));
```

### Application Insights (Azure)

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

---

## 🆘 Precisa de Mais Ajuda?

1. Procure na [Documentation](https://docs.microsoft.com/aspnet/core)
2. Abra issue no [GitHub](https://github.com/laragoncalvs/PetStay/issues)
3. Procure no [Stack Overflow](https://stackoverflow.com/questions/tagged/asp.net-core)
4. Comunidade [Microsoft Docs](https://learn.microsoft.com/dotnet)

---

**Última atualização:** Janeiro 2025
