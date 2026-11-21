# 📚 Documentação - PetStay

Bem-vindo à documentação completa do PetStay! Use os links abaixo para navegar.

## 🚀 Começar Rápido

| Arquivo | Descrição |
|---------|-----------|
| **[README.md](README.md)** | Overview do projeto e requisitos |
| **[SETUP.md](SETUP.md)** | Instruções detalhadas por SO |
| **[setup.sh](setup.sh)** | Script automático (Linux/macOS) |
| **[setup.bat](setup.bat)** | Script automático (Windows) |

### Primeiro Acesso?
👉 Comece pelo [README.md](README.md)

---

## 🛠️ Desenvolvimento

| Arquivo | Descrição |
|---------|-----------|
| **[CONTRIBUTING.md](CONTRIBUTING.md)** | Como contribuir com código |
| **[TROUBLESHOOTING.md](TROUBLESHOOTING.md)** | FAQ e solução de problemas |
| **[.env.example](.env.example)** | Variáveis de ambiente |

---

## 📖 Documentação Técnica

### Arquitetura
```
├── Controllers/     → Endpoints e lógica MVC
├── Models/         → Entidades de domínio
├── Views/          → Interface com usuário
├── Services/       → Lógica de negócio
├── Data/           → Acesso a dados
└── Migrations/     → Histórico do banco
```

### Stack Tecnológico
- **Backend**: ASP.NET Core 10
- **Frontend**: Razor + Bootstrap 5
- **Database**: SQL Server + Entity Framework Core
- **Auth**: ASP.NET Core Identity

---

## 🚦 Fluxo de Setup

### Windows
```
1. .NET 10 SDK
   ↓
2. SQL Server Express
   ↓
3. setup.bat
   ↓
4. dotnet run
```

### Linux/macOS
```
1. .NET 10 SDK
2. (Opcional: SQL Server)
   ↓
3. bash setup.sh
   ↓
4. dotnet run
```

### Docker
```
1. Docker Desktop
   ↓
2. docker-compose up
   ↓
3. http://localhost:8080
```

---

## 🔑 Recursos Principais

### 👥 Autenticação
- Registro de usuários
- Login/Logout
- 3 roles: Tutor, Gestor, Administrador
- Password validation forte

### 🏨 Gerenciamento de Hotéis
- Cadastro de hotéis
- Aprovação administrativa
- Gestão por proprietário

### 🐾 Gestão de Pets
- Cadastro de pets por tutor
- Informações: raça, peso, data nascimento

### 📅 Sistema de Reservas
- Criação de reservas
- Status: Pendente → Confirmada → EmProgresso → Finalizada
- Check-in/Check-out gerenciado
- Cancelamento automático após 24h de pendência

### ⭐ Avaliações
- Sistema de reviews pós-hospedagem
- Nota e comentário

---

## 🍕 Primeiros Passos

### 1. Setup Local (5 min)
```bash
git clone https://github.com/laragoncalvs/PetStay.git
cd PetStay
bash setup.sh  # ou setup.bat
dotnet run
```

### 2. Acessar Admin
- Email: `admin@petsstay.com`
- Senha: `Admin@123`
⚠️ **Altere na primeira execução!**

### 3. Explorar Features
- Crie um hotel
- Adicione um pet
- Faça uma reserva
- Gerencie check-in/out

---

## 📊 Estrutura de Dados

### Usuários
```
ApplicationUser
├── Id (PK)
├── Email
├── Perfil (Tutor|Gestor|Admin)
├── Ativo (bool)
└── Relacionamentos
	├── Pets (1→N)
	├── Hotéis (como Gestor)
	└── Reservas (como Tutor)
```

### Hotéis
```
Hotel
├── Id (PK)
├── Nome
├── Diária (decimal)
├── GestorId (FK)
├── Aprovado (bool)
└── Pets (1→N)
```

### Reservas
```
Reservation
├── Id (PK)
├── HotelId (FK)
├── PetId (FK)
├── TutorId (FK)
├── Status (enum)
├── DataEntrada/Saída
├── DataCheckInEfetiva/CheckOutEfetiva
├── ValorTotal (decimal)
└── Observações
```

---

## 🔐 Segurança

- [x] Password hashing (bcrypt)
- [x] HTTPS
- [x] CSRF protection
- [x] SQL Injection prevention (EF Core)
- [x] XSS protection
- [x] Authorization policies
- [ ] 2FA (TODO)
- [ ] Rate limiting (TODO)

---

## 🚀 Deployment

### Azure
```bash
1. Criar banco SQL
2. Criar App Service
3. Publicar com git push
4. Configurar variáveis de ambiente
```

### Heroku
Não suportado (requer SQL Server)

### Self-hosted
```bash
1. Servidor Linux/Windows
2. .NET Runtime
3. SQL Server
4. dotnet publish + systemd service
```

---

## 📞 Suporte

| Canal | Link |
|-------|------|
| **Issues** | [GitHub Issues](https://github.com/laragoncalvs/PetStay/issues) |
| **Discussions** | [GitHub Discussions](https://github.com/laragoncalvs/PetStay/discussions) |
| **Email** | contrib@petsstay.com (TODO) |

---

## 🤝 Versioning

- **Versão atual**: 1.0.0
- **Status**: Beta
- **Última atualização**: Janeiro 2025

---

## 📄 Licença

MIT License - veja [LICENSE](LICENSE)

---

## 🎯 Roadmap

- [ ] Integração com pagamento (Stripe/PayPal)
- [ ] Notificações por email/SMS
- [ ] Aplicativo mobile
- [ ] Sistema de pontos/cashback
- [ ] Integração com mapa
- [ ] Chat em tempo real
- [ ] Relatórios avançados

---

**Precisa expandir algo?**

Cada arquivo tem mais detalhes:
- Clique no arquivo no índice acima
- Ou em [GitHub](https://github.com/laragoncalvs/PetStay)

**Bom desenvolvimento! 🚀**
