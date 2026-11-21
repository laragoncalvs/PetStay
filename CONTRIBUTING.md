# Guia de Contribuição - PetStay

Obrigado por se interessar em contribuir com PetStay! 🎉

## Como Contribuir

### 1. Fork o Repositório

Faça um fork do projeto no GitHub:
https://github.com/laragoncalvs/PetStay

### 2. Clone seu Fork

```bash
git clone https://github.com/seu-usuario/PetStay.git
cd PetStay
git remote add upstream https://github.com/laragoncalvs/PetStay.git
```

### 3. Crie uma Branch

```bash
git checkout -b feature/sua-feature
# ou
git checkout -b fix/seu-bug-fix
```

**Convenção de nomes:**
- `feature/*` - Novas funcionalidades
- `fix/*` - Correções de bugs
- `docs/*` - Documentação
- `refactor/*` - Refatoração de código

### 4. Faça suas Alterações

```bash
# Faça alterações no código
dotnet build
dotnet run
```

### 5. Commit

```bash
git add .
git commit -m "Descrição clara da mudança"
```

**Formato de commit:**
```
tipo(escopo): descrição

corpo (opcional)

rodapé (opcional)
```

Exemplos:
- `feat(reservation): adicionar filtro por status`
- `fix(auth): corrigir bug de login duplicado`
- `docs(readme): atualizar instruções de setup`
- `refactor(models): melhorar estrutura de entidades`

### 6. Push para seu Fork

```bash
git push origin feature/sua-feature
```

### 7. Abra um Pull Request

No GitHub, clique em "New Pull Request" e descreva suas alterações:

- **Título**: Descritivo e conciso
- **Descrição**: Explique o problema e a solução
- **Screenshots**: Se for UI, adicione capturas
- **Testes**: Mencione testes realizados

---

## Padrões de Código

### C#
```csharp
// Nomenclatura
public class UserService { } // PascalCase para classes
private readonly AppDbContext _context; // _camelCase para privado
public async Task<IActionResult> GetUsers() { } // PascalCase para métodos

// Naming conventions
- Classes: PascalCase
- Methods: PascalCase
- Private fields: _camelCase
- Local variables: camelCase
- Constants: UPPER_SNAKE_CASE

// Async/Await
public async Task<IActionResult> Create(Model model)
{
	var result = await _service.ProcessAsync(model);
	return Ok(result);
}

// Null coalescing
var name = user?.Name ?? "Unknown";

// Using statements
using var connection = new SqlConnection(connectionString);
```

### Razor Views
```html
<!-- PascalCase para componentes -->
<div class="container">
	@if (Model.Any())
	{
		@foreach (var item in Model)
		{
			<div class="item">
				@item.Name
			</div>
		}
	}
</div>
```

### Banco de Dados
```sql
-- UPPER_SNAKE_CASE para nomes de colunas/tabelas
CREATE TABLE Users (
	Id INT PRIMARY KEY,
	User_Name NVARCHAR(100),
	Email NVARCHAR(100)
)
```

---

## Estrutura de Diretórios

```
PetStay/
├── Controllers/        # Controllers MVC
├── Views/             # Views Razor
├── Models/            # Entidades + ViewModels
│   ├── DTOs/         # Data Transfer Objects
│   └── ViewModels/   # ViewModels
├── Data/              # DbContext + Configurações
├── Services/          # Lógica de negócio
├── Migrations/        # Entity Framework Migrations
├── wwwroot/          # Assets estáticos
└── Tests/            # Testes (se houver)
```

**Ao adicionar novos arquivos, respeite esta estrutura!**

---

## Checklist antes de Submeter PR

- [ ] Código compilava sem erros
- [ ] Segue as convenções de nomes
- [ ] Adicionar comentários em lógica complexa
- [ ] Sem código comentado ou console.log/Debug
- [ ] Testar manualmente a funcionalidade
- [ ] Atualizar documentação se necessário
- [ ] Sem alterações de `appsettings.json` (senhas/dados sensíveis)
- [ ] Commits com mensagens claras

---

## Relatar Bugs

Use o GitHub Issues:

1. **Verificar** se o bug não foi reportado
2. **Descrever** o problema claramente
3. **Reproduzir** com passos específicos
4. **Screenshots/Logs** quando possível
5. **Ambiente**: SO, versão .NET, etc

### Exemplo:
```markdown
**Descrição:**
Ao fazer login, a aplicação redireciona para página em branco

**Passos para reproduzir:**
1. Ir para login
2. Inserir credenciais válidas
3. Clicar em "Entrar"

**Comportamento esperado:**
Redirecionar para dashboard

**Ambiente:**
- Windows 11
- .NET 10.0.0
- SQL Server 2022
```

---

## Sugerir Features

Abra uma issue com o tag `enhancement`:

```markdown
**Descrição:**
Adicionar filtro por data em reservas

**Motivação:**
Facilitaria buscar reservas em períodos específicos

**Solução proposta:**
Adicionar campo de data range no filtro

**Alternativas:**
Pode ser um dropdown com períodos pré-definidos
```

---

## Dúvidas ou Discussões?

1. Abra uma [Discussion](https://github.com/laragoncalvs/PetStay/discussions)
2. Mencione os maintainers
3. Descreva sua dúvida claramente

---

## Reconhecimento

Todos os contribuidores serão reconhecidos em:
- README.md
- Página de contribuidores do GitHub

---

**Obrigado por contribuir! ❤️**
