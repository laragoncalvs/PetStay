using System.ComponentModel.DataAnnotations;

namespace PetStay.Models
{
    public enum TamanhoPet
    {
        Pequeno = 1,
        Medio = 2,
        Grande = 3
    }

    public enum FaixaEtariaPet
    {
        Filhote = 1,
        Adulto = 2,
        Idoso = 3
    }

    public class Pet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(80)]
        public string Especie { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Raca { get; set; } = string.Empty;

        public TamanhoPet Tamanho { get; set; }

        public string? Porte { get; set; } // Campo compatível com Confirmation view

        public FaixaEtariaPet FaixaEtaria { get; set; }

        public DateTime DataNascimento { get; set; }

        public decimal Peso { get; set; }

        [StringLength(1000)]
        public string? CuidadosEspecificos { get; set; }

        public bool DeclaracaoVacinacao { get; set; }

        public int TutorId { get; set; }

        public ApplicationUser Tutor { get; set; } = null!;
    }
}
