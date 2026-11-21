using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStay.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int HotelId { get; set; }

        public Hotel Hotel { get; set; } = null!;

        [Required]
        public int TutorId { get; set; }

        public ApplicationUser Tutor { get; set; } = null!;

        [Required]
        public int ReservationId { get; set; }

        public Reservation Reservation { get; set; } = null!;

        [Required]
        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5 estrelas.")]
        public int Nota { get; set; }

        [StringLength(500, ErrorMessage = "O comentário não pode ter mais de 500 caracteres.")]
        public string? Comentario { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? TituloAvaliacao { get; set; }
    }
}
