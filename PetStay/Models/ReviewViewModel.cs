using System.ComponentModel.DataAnnotations;

namespace PetStay.Models
{
    public class ReviewViewModel
    {
        public int ReservationId { get; set; }

        public int HotelId { get; set; }

        public string? HotelNome { get; set; }

        [Required(ErrorMessage = "A nota é obrigatória.")]
        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5 estrelas.")]
        public int Nota { get; set; }

        [StringLength(100, ErrorMessage = "O título não pode ter mais de 100 caracteres.")]
        public string? TituloAvaliacao { get; set; }

        [StringLength(500, ErrorMessage = "O comentário não pode ter mais de 500 caracteres.")]
        public string? Comentario { get; set; }

        // Informações da reserva para contexto
        public string? PetNome { get; set; }

        public DateTime DataEntrada { get; set; }

        public DateTime DataSaida { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
