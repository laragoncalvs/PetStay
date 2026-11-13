using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStay.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do hotel é obrigatório.")]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        [StringLength(250)]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [StringLength(100)]
        public string Cidade { get; set; } = string.Empty;

        [StringLength(2)]
        public string? Estado { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Telefone { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(1000)]
        public string? Descricao { get; set; }

        [Range(typeof(decimal), "0", "5", ErrorMessage = "A avaliação deve estar entre 0 e 5.")]
        [Column(TypeName = "decimal(2,1)")]
        public decimal Avaliacao { get; set; }

        [Url(ErrorMessage = "Informe uma URL válida para a imagem.")]
        [StringLength(500)]
        public string? ImagemUrl { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser maior que zero.")]
        public int Capacidade { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Diaria { get; set; }

        public bool Ativo { get; set; } = true;
    }
}
