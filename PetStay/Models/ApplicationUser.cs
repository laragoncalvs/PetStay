using Microsoft.AspNetCore.Identity;

namespace PetStay.Models
{
    public enum PerfilUsuario
    {
        Tutor = 1,
        Gestor = 2,
        Administrador = 3
    }

    public class ApplicationUser : IdentityUser<int>
    {
        public string Nome { get; set; } = string.Empty;

        public PerfilUsuario Perfil { get; set; }

        public bool TermosAceitos { get; set; }

        public DateTime TermosAceitosEm { get; set; }

        public bool Ativo { get; set; } = true;

        public int? GestorDonoId { get; set; }
        public ApplicationUser? GestorDono { get; set; }
        public ICollection<ApplicationUser> GestoresFuncionarios { get; set; } = new List<ApplicationUser>();


        public ICollection<Pet> Pets { get; set; } = new List<Pet>();

        public ICollection<Hotel> Hoteis { get; set; } = new List<Hotel>();

        public ICollection<Hotel> HoteisAnalisados { get; set; } = new List<Hotel>();
    }
}
