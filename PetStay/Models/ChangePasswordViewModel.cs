using System.ComponentModel.DataAnnotations;

namespace PetStay.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Informe sua senha atual.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha atual")]
        public string SenhaAtual { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a nova senha.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        public string NovaSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme a nova senha.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NovaSenha), ErrorMessage = "As senhas não conferem.")]
        [Display(Name = "Confirme a nova senha")]
        public string ConfirmacaoNovaSenha { get; set; } = string.Empty;
    }
}
