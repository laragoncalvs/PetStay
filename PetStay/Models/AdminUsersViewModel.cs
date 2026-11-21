namespace PetStay.Models
{
    public class AdminUsersViewModel
    {
        public string? Busca { get; set; }

        public IReadOnlyList<AdminUserItemViewModel> Usuarios { get; set; } = Array.Empty<AdminUserItemViewModel>();
    }

    public class AdminUserItemViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public PerfilUsuario Perfil { get; set; }

        public bool Ativo { get; set; }

        public bool EhUsuarioAtual { get; set; }
    }
}
