using System.ComponentModel.DataAnnotations;

namespace PetStay.Models
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Informe seu nome.")]
        [StringLength(150)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe seu telefone.")]
        [Phone(ErrorMessage = "Informe um telefone válido.")]
        [StringLength(20)]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; } = string.Empty;
    }
}
