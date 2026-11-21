using System.ComponentModel.DataAnnotations;

namespace PetStay.Models.DTOs
{
    public class PaymentProcessingDto
    {
        [Required]
        public int ReservationId { get; set; }

        [Required(ErrorMessage = "Selecione um método de pagamento")]
        public required string MetodoPagamento { get; set; }

        // Dados para cartão
        public string? NomeCartao { get; set; }
        public string? NumeroCartao { get; set; }
        public string? ValidadeCartao { get; set; }
        public string? CvvCartao { get; set; }
    }
}
