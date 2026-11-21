# PetStay 🐾

Sistema de gerenciamento de reservas para hotéis de pets, desenvolvido em ASP.NET Core 10.

## 📋 Requisitos

- **SDK .NET 10.0+** - [Download aqui](https://dotnet.microsoft.com/download/dotnet/10.0)

## 🚀 Como Rodar (3 passos)

### 1. Clonar e Restaurar
```bash
git clone https://github.com/laragoncalvs/PetStay.git
cd PetStay
dotnet restore
```

### 2. Executar
```bash
dotnet run
```

### 3. Abrir no Navegador
```
http://localhost:5000
```

## 🔐 Credenciais Padrão

- **Email**: `admin@petstay.com`
- **Senha**: `Teste1234!`

## 📁 Estrutura do Projeto

```
PetStay/
├── Controllers/     # Lógica da aplicação
├── Models/         # Dados e entidades
├── Views/          # Interface gráfica
├── Data/           # Banco de dados
├── Services/       # Serviços
└── wwwroot/        # CSS, JS, imagens
```


## 🛠️ Tecnologias

- ASP.NET Core 10
- Entity Framework Core
- SQL Server (Azure)
- Bootstrap 5
- ASP.NET Core Identity

## 📞 Dúvidas?

Qualquer erro, abra issue no GitHub ou tente:
```bash
dotnet restore
dotnet build
dotnet run
```
