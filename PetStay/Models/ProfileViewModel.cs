namespace PetStay.Models
{
    public class ProfileViewModel
    {
        public EditProfileViewModel DadosPessoais { get; set; } = new();

        public ChangePasswordViewModel AlteracaoSenha { get; set; } = new();
    }
}
