using System.ComponentModel.DataAnnotations;

namespace PetStay.Models
{
    public class PetCreateViewModel
    {
        [Required(ErrorMessage = "Informe o nome do pet.")]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a espécie do pet.")]
        [StringLength(80)]
        [Display(Name = "Espécie")]
        public string Especie { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a raça do pet.")]
        [StringLength(100)]
        [Display(Name = "Raça")]
        public string Raca { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione o tamanho do pet.")]
        [Display(Name = "Tamanho")]
        public TamanhoPet? Tamanho { get; set; }

        [Required(ErrorMessage = "Selecione a faixa etária do pet.")]
        [Display(Name = "Faixa etária")]
        public FaixaEtariaPet? FaixaEtaria { get; set; }

        [StringLength(1000, ErrorMessage = "Os cuidados específicos devem ter no máximo 1000 caracteres.")]
        [Display(Name = "Cuidados específicos")]
        public string? CuidadosEspecificos { get; set; }

        [Display(Name = "Declaro que a vacinação está em dia")]
        public bool DeclaracaoVacinacao { get; set; }
    }
}
