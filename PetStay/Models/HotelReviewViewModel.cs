using System.ComponentModel.DataAnnotations;

namespace PetStay.Models
{
    public enum DecisaoHotel
    {
        Aprovado = 1,
        Reprovado = 2
    }

    public class HotelReviewViewModel
    {
        public int HotelId { get; set; }

        public string NomeHotel { get; set; } = string.Empty;

        public string? Cidade { get; set; }

        public string? GestorNome { get; set; }

        [Required(ErrorMessage = "Selecione uma decisão.")]
        [Display(Name = "Decisão")]
        public DecisaoHotel? Decisao { get; set; }

        [Required(ErrorMessage = "Informe a justificativa da decisão.")]
        [StringLength(1000, ErrorMessage = "A justificativa deve ter no máximo 1000 caracteres.")]
        [Display(Name = "Justificativa")]
        public string Justificativa { get; set; } = string.Empty;
    }
}
