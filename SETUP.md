# 🔧 Guia Detalhado de Configuração - PetStay

Este guia fornece instruções passo-a-passo para configurar e rodar o PetStay em diferentes ambientes.

## 📋 Índice
1. [Windows (Local)](#windows-local)
2. [Linux (Local)](#linux-local)
3. [macOS (Local)](#macos-local)
4. [Docker](#docker)
5. [Azure Cloud](#azure-cloud)

---

## Windows (Local)

### Pré-requisitos
- Windows 10+
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server 2019+](https://www.microsoft.com/pt-br/sql-server/sql-server-2019) ou SQL Server Express
- [Git](https://git-scm.com/)

### Passo 1: Instalar .NET 10

```powershell
# Verificar instalação
dotnet --version
```

### Passo 2: Instalar SQL Server Express (se não tiver)

Baixe em: https://www.microsoft.com/pt-br/sql-server/sql-server-express

Padrões durante instalação:
- Named Instance: `SQLEXPRESS`
- Authentication: Mixed Mode (Windows + SQL)

### Passo 3: Configurar String de Conexão

Abra `PetStay/appsettings.json`:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=.\\SQLEXPRESS;Database=PetStayDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Passo 4: Clonar e Configurar

```powershell
git clone https://github.com/laragoncalvs/PetStay.git
cd PetStay
dotnet restore
```

### Passo 5: Criar Banco de Dados

```powershell
dotnet ef database update
```

### Passo 6: Executar

```powershell
dotnet run
```

Abra: http://localhost:5000

---

## Linux (Local)

### Pré-requisitos

#### Ubuntu/Debian:
```bash
# Atualizar pacotes
sudo apt update && sudo apt upgrade -y

# Instalar .NET 10
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0

# Adicionar ao PATH
export PATH=$PATH:$HOME/.dotnet
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc

# Instalar SQL Server (opcional - use nuvem se preferir)
sudo apt install -y mssql-server

# Instalar Git
sudo apt install -y git
```

#### Fedora:
```bash
sudo dnf install -y dotnet-sdk-10.0 git
```

### Passo 1: Verificar Instalação

```bash
dotnet --version
git --version
```

### Passo 2: Usar Banco em Nuvem (Recomendado)

Como não é prático rodar SQL Server no Linux localmente, use Azure:

Edite `PetStay/appsettings.json`:
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=tcp:seu-servidor.database.windows.net,1433;Initial Catalog=PetStayDb;User ID=seu-usuario;Password=sua-senha;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

### Passo 3: Clonar e Configurar

```bash
git clone https://github.com/laragoncalvs/PetStay.git
cd PetStay
dotnet restore
```

### Passo 4: Aplicar Migrations

```bash
dotnet ef database update
```

### Passo 5: Executar

```bash
dotnet run
```

Acesse: http://localhost:5000

---

## macOS (Local)

### Pré-requisitos

```bash
# Instalar Homebrew (se não tiver)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Instalar dependências
brew install dotnet git
```

### Passo 1: Verificar .NET

```bash
dotnet --version
```

### Passo 2: Configurar Banco

Use Azure SQL Database (recomendado para macOS):

Edite `PetStay/appsettings.json`:
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=tcp:seu-servidor.database.windows.net,1433;Initial Catalog=PetStayDb;User ID=seu-usuario;Password=sua-senha;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

### Passo 3: Configurar Projeto

```bash
git clone https://github.com/laragoncalvs/PetStay.git
cd PetStay
dotnet restore
dotnet ef database update
dotnet run
```

Acesse: http://localhost:5000

---

## 🐳 Docker

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Passo 1: Criar Dockerfile

Crie arquivo `Dockerfile` na raiz do projeto:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "PetStay.dll"]
```

### Passo 2: Criar docker-compose.yml

```yaml
version: '3.8'
services:
  sql-server:
	image: mcr.microsoft.com/mssql/server:2022-latest
	environment:
	  SA_PASSWORD: "YourPassword123!"
	  ACCEPT_EULA: "Y"
	ports:
	  - "1433:1433"
	volumes:
	  - sqlserver_data:/var/opt/mssql

  petsstay:
	build: .
	ports:
	  - "8080:80"
	environment:
	  ConnectionStrings__DefaultConnection: "Server=sql-server;Database=PetStayDb;User Id=sa;Password=YourPassword123!;"
	depends_on:
	  - sql-server
	volumes:
	  - .:/app

volumes:
  sqlserver_data:
```

### Passo 3: Executar com Docker Compose

```bash
docker-compose up
```

Acesse: http://localhost:8080

---

## ☁️ Azure Cloud

### Pré-requisitos

- Conta na [Azure](https://azure.microsoft.com)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)

### Passo 1: Criar Banco de Dados SQL

```bash
# Login
az login

# Criar Resource Group
az group create --name PetStayRG --location brazilsouth

# Criar SQL Server
az sql server create \
  --name petstay-server \
  --resource-group PetStayRG \
  --admin-user sqladmin \
  --admin-password "ComplexPassword123!"

# Criar Database
az sql db create \
  --resource-group PetStayRG \
  --server petstay-server \
  --name PetStayDb \
  --edition Basic
```

### Passo 2: Obter String de Conexão

```bash
az sql db show-connection-string \
  --name PetStayDb \
  --server petstay-server \
  --client ado.net
```

### Passo 3: Atualizar appsettings.json

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=tcp:petstay-server.database.windows.net,1433;Initial Catalog=PetStayDb;Persist Security Info=False;User ID=sqladmin;Password=ComplexPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

### Passo 4: Publicar com Azure App Service

```bash
# Criar App Service
az appservice plan create \
  --name PetStayPlan \
  --resource-group PetStayRG \
  --sku B1 --is-linux

az webapp create \
  --resource-group PetStayRG \
  --plan PetStayPlan \
  --name petstay-app \
  --runtime "DOTNETCORE|10.0"

# Publicar
dotnet publish -c Release
az webapp deployment source config-zip \
  --resource-group PetStayRG \
  --name petstay-app \
  --src bin/Release/net10.0/publish.zip
```

Acesse: https://petstay-app.azurewebsites.net

---

## 🔒 Segurança

### Armazenar Senhas com Segurança

**Nunca commite senhas no repositório!**

Use `User Secrets` para desenvolvimento:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "sua-string-de-conexao"
```

### Variáveis de Ambiente em Produção

Use variáveis de ambiente do seu platform (Azure, AWS, etc).

---

## ✅ Checklist de Configuração

- [ ] .NET 10 instalado
- [ ] SQL Server (local ou nuvem) configurado
- [ ] Git instalado
- [ ] Projeto clonado
- [ ] Packages restaurados (`dotnet restore`)
- [ ] Migrations aplicadas (`dotnet ef database update`)
- [ ] Aplicação rodando (`dotnet run`)
- [ ] Banco de dados criado
- [ ] Usuário admin criado

---

## 📞 Ajuda

Se encontrar problemas:

1. Execute `dotnet restore` novamente
2. Verifique conexão com banco: `telnet localhost 1433`
3. Limpe cache: `dotnet nuget locals all --clear`
4. Recompile: `dotnet clean && dotnet build`
5. Abra issue no GitHub com o erro

---

**Tudo pronto? Divirta-se desenvolvendo! 🎉**
