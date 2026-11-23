using System.ComponentModel.DataAnnotations;

namespace PetStay.Models
{
    public class HotelCreateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "O nome do hotel é obrigatório.")]
        [StringLength(150)]
        [Display(Name = "Nome do hotel")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        [StringLength(250)]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [StringLength(100)]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; } = string.Empty;

        [StringLength(2)]
        [Display(Name = "Estado")]
        public string? Estado { get; set; }

        [Phone(ErrorMessage = "Informe um telefone válido.")]
        [StringLength(20)]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [StringLength(150)]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [StringLength(1000)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser maior que zero.")]
        [Display(Name = "Capacidade de pets")]
        public int Capacidade { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Informe uma diária válida.")]
        [Display(Name = "Diária")]
        public decimal Diaria { get; set; }

        
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true, ErrorMessage = "Informe uma diária válida para pets pequenos.")]
        [Display(Name = "Diária para porte pequeno")]
        public decimal? DiariaPequeno { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true, ErrorMessage = "Informe uma diária válida para pets médios.")]
        [Display(Name = "Diária para porte médio")]
        public decimal? DiariaMedio { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true, ErrorMessage = "Informe uma diária válida para pets grandes.")]
        [Display(Name = "Diária para porte grande")]
        public decimal? DiariaGrande { get; set; }


        [Url(ErrorMessage = "Informe uma URL válida para a imagem.")]
        [StringLength(500)]
        [Display(Name = "URL da imagem")]
        public string? ImagemUrl { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Abertura")]
        public TimeSpan HorarioAbertura { get; set; } = new(8, 0, 0);

        [DataType(DataType.Time)]
        [Display(Name = "Fechamento")]
        public TimeSpan HorarioFechamento { get; set; } = new(18, 0, 0);

        [MinLength(1, ErrorMessage = "Selecione pelo menos um porte aceito.")]
        [Display(Name = "Portes aceitos")]
        public List<TamanhoPet> PortesAceitos { get; set; } = new();

        [MinLength(1, ErrorMessage = "Selecione pelo menos uma faixa etária aceita.")]
        [Display(Name = "Faixas etárias aceitas")]
        public List<FaixaEtariaPet> FaixasEtariasAceitas { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HorarioFechamento <= HorarioAbertura)
            {
                yield return new ValidationResult(
                    "O horário de fechamento deve ser posterior ao horário de abertura.",
                    new[] { nameof(HorarioFechamento) });
            }

            if (PortesAceitos.Contains(TamanhoPet.Pequeno) && !DiariaPequeno.HasValue)
                yield return new ValidationResult("Informe a diária para pets pequenos.", new[] { nameof(DiariaPequeno) });

            if (PortesAceitos.Contains(TamanhoPet.Medio) && !DiariaMedio.HasValue)
                yield return new ValidationResult("Informe a diária para pets médios.", new[] { nameof(DiariaMedio) });

            if (PortesAceitos.Contains(TamanhoPet.Grande) && !DiariaGrande.HasValue)
                yield return new ValidationResult("Informe a diária para pets grandes.", new[] { nameof(DiariaGrande) });

        }
    }
}
