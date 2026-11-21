using System.ComponentModel.DataAnnotations;

namespace PetStay.Models
{
    public class PaymentViewModel
    {
        public int ReservationId { get; set; }

        public decimal ValorTotal { get; set; }

        public string? HotelNome { get; set; }

        public string? PetNome { get; set; }

        public DateTime DataEntrada { get; set; }

        public DateTime DataSaida { get; set; }

        [Required(ErrorMessage = "Selecione um método de pagamento.")]
        public string? MetodoPagamento { get; set; } // "pix", "cartao_credito", "cartao_debito"

        // Dados para cartão (apenas aplicável se MetodoPagamento for cartão)
        [StringLength(50)]
        public string? NomeCartao { get; set; }

        [RegularExpression(@"^\d{16}$", ErrorMessage = "Número do cartão deve ter 16 dígitos.")]
        public string? NumeroCartao { get; set; }

        [RegularExpression(@"^\d{2}/\d{2}$", ErrorMessage = "Validade deve estar no formato MM/AA.")]
        public string? ValidadeCartao { get; set; }

        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV deve ter 3 dígitos.")]
        public string? CvvCartao { get; set; }

        // Dados para PIX (mock - sempre gera um código para demo)
        public string? CodigoPixCopia { get; set; }

        public string? QrCodePixUrl { get; set; }
    }
}
