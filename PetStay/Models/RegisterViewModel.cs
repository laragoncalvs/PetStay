using System.ComponentModel.DataAnnotations;

namespace PetStay.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Informe seu nome.")]
        [StringLength(150)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe seu e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe seu telefone.")]
        [Phone(ErrorMessage = "Informe um telefone válido.")]
        [StringLength(20)]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione o tipo de cadastro.")]
        [Display(Name = "Cadastrar como")]
        public PerfilUsuario? Perfil { get; set; }

        [Required(ErrorMessage = "Informe uma senha.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme sua senha.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Senha), ErrorMessage = "As senhas não conferem.")]
        [Display(Name = "Confirme a senha")]
        public string ConfirmacaoSenha { get; set; } = string.Empty;

        [Range(typeof(bool), "true", "true", ErrorMessage = "É necessário aceitar os termos de uso.")]
        [Display(Name = "Aceito os termos de uso")]
        public bool AceitouTermos { get; set; }
    }
}
