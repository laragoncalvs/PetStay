# PetStay 🐾

Uma plataforma moderna de gerenciamento de reservas para hotéis e pensionatos de pets, permitindo tutores agendar hospedagem para seus animais e proprietários de hotéis gerenciarem as reservas de forma eficiente.

## 📋 Requisitos do Sistema

Antes de rodar o projeto, certifique-se de ter instalado:

- **SDK .NET 10.0+** - [Download aqui](https://dotnet.microsoft.com/download/dotnet/10.0)
- **SQL Server 2019+** ou **Azure SQL Database**
- **Git** - [Download aqui](https://git-scm.com/)
- **Visual Studio 2022+** (opcionalmente) ou qualquer editor de código

Verifique suas versões:
```bash
dotnet --version
sqlcmd -v
git --version
```

## 🚀 Como Rodar o Projeto

### 1. Clonar o Repositório

```bash
git clone https://github.com/laragoncalvs/PetStay.git
cd PetStay
```

### 2. Restaurar Dependências

```bash
dotnet restore
```

### 3. Configurar Banco de Dados

#### Opção A: Usar SQL Server Local

1. Abra o arquivo `appsettings.json` e atualize a string de conexão:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=PetStayDb;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

Ajuste conforme sua configuração local:
- `Server=` → Nome do seu servidor (`.` para local)
- `Database=` → Nome do banco de dados
- `User ID=` e `Password=` → Se não usar Windows Authentication

#### Opção B: Usar Banco em Nuvem (Azure SQL Database)

A string de conexão já está configurada para Azure SQL. Certifique-se de ter as credenciais corretas em `appsettings.json`.

### 4. Aplicar Migrations (Criar/Atualizar Banco)

```bash
dotnet ef database update
```

Este comando irá:
- Criar o banco de dados (se não existir)
- Executar todas as migrations
- Criar um usuário administrador padrão

### 5. Executar a Aplicação

```bash
dotnet run
```

A aplicação estará disponível em:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001

## 🔐 Credenciais Padrão

Após a primeira execução, um usuário administrador é criado automaticamente:

- **Email**: `admin@petsstay.com`
- **Senha**: `Admin@123`

⚠️ **Importante**: Altere essa senha imediatamente em produção!

## 📁 Estrutura do Projeto

```
PetStay/
├── Controllers/          # Controllers (API endpoints e ações web)
├── Models/              # Entidades e ViewModels
├── Views/               # Views Razor MVC
├── Data/                # Contexto do Entity Framework
├── Migrations/          # Migrações do banco de dados
├── Services/            # Serviços de negócio
├── wwwroot/             # Arquivos estáticos (CSS, JS, imagens)
├── appsettings.json     # Configurações da aplicação
└── Program.cs           # Startup da aplicação
```

## 🛠️ Tecnologias Utilizadas

- **Backend**: ASP.NET Core 10.0
- **Frontend**: Razor Pages + Bootstrap 5
- **Database**: SQL Server + Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **ORM**: Entity Framework Core

## 🔧 Variáveis de Ambiente

Você pode sobrescrever configurações usando variáveis de ambiente:

```bash
# Linux/Mac
export ConnectionStrings__DefaultConnection="your-connection-string"
export ASPNETCORE_ENVIRONMENT="Production"

# Windows (PowerShell)
$env:ConnectionStrings__DefaultConnection="your-connection-string"
$env:ASPNETCORE_ENVIRONMENT="Production"

dotnet run
```

## 📦 Packages NuGet

Os seguintes pacotes são utilizados:

- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` - Gerenciamento de usuários
- `Microsoft.EntityFrameworkCore.SqlServer` - Provider do SQL Server
- `Microsoft.EntityFrameworkCore.Design` - Ferramentas de design e migrations

Para atualizar:
```bash
dotnet add package <nome-do-pacote> --version <versao>
```

## 🔄 Migrations (Atualizar Banco de Dados)

Ao fazer alterações no modelo de dados:

```bash
# Criar uma nova migration
dotnet ef migrations add NomeDaMigracao

# Aplicar ao banco
dotnet ef database update

# Remover última migration (se necessário)
dotnet ef migrations remove

# Ver histórico de migrations
dotnet ef migrations list
```

## 🌍 Ambientes

A aplicação suporta múltiplos ambientes:

- **Development** - Desenvolvimento com detalhes de erro completos
- **Production** - Produção com otimizações

Configure com:
```bash
dotnet run --environment Production
```

Ou via arquivo `Properties/launchSettings.json`

## 🧪 Rodando Testes (se houver)

```bash
dotnet test
```

## 📝 Configurações Importantes

### appsettings.json

```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.AspNetCore": "Warning"
	}
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
	"DefaultConnection": "Sua string de conexão aqui"
  }
}
```

### appsettings.Development.json

Use este arquivo para configurações de desenvolvimento local (não commitar senhas!).

## 🐛 Solução de Problemas

### Erro: "Database connection failed"
- Verifique se o SQL Server está rodando
- Valide a string de conexão
- Confirme credenciais de acesso

### Erro: "Migrations pending"
```bash
dotnet ef database update
```

### Erro: "Port 5000/5001 already in use"
```bash
dotnet run --urls "http://localhost:3000"
```

### Erro ao restaurar packages
```bash
dotnet nuget locals all --clear
dotnet restore
```

## 📚 Documentação Adicional

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)

## 👥 Roles e Permissões

A aplicação possui os seguintes perfis de usuário:

- **Tutor**: Usuário comum que reserva hospedagem
- **Gestor**: Gerencia um ou mais hotéis
- **Administrador**: Acesso completo à plataforma

## 📞 Suporte e Contribuição

Para reportar issues ou sugerir melhorias, abra uma issue no repositório:
https://github.com/laragoncalvs/PetStay/issues

## 📄 Licença

Este projeto é licenciado sob a [MIT License](LICENSE) - veja o arquivo LICENSE para detalhes.

---

**Desenvolvido com ❤️ para os amigos peludos** 🐶🐱
