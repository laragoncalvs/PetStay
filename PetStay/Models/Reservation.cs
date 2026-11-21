using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStay.Models
{
    public enum StatusReserva
    {
        Pendente = 0,
        Confirmada = 1,
        EmProgresso = 2,
        Finalizada = 3,
        Cancelada = 4
    }

    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public int PetId { get; set; }

        public Pet Pet { get; set; } = null!;

        [Required]
        public int HotelId { get; set; }

        public Hotel Hotel { get; set; } = null!;

        [Required]
        public int TutorId { get; set; }

        public ApplicationUser Tutor { get; set; } = null!;

        [Required]
        public DateTime DataEntrada { get; set; }

        [Required]
        public DateTime DataSaida { get; set; }

        // Horários específicos de check-in e check-out
        public TimeSpan? HorarioEntrada { get; set; }

        public TimeSpan? HorarioSaida { get; set; }

        public StatusReserva Status { get; set; } = StatusReserva.Pendente;

        public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

        // Data quando a reserva foi solicitada (para rastrear 24h)
        public DateTime SolicitadaEm { get; set; } = DateTime.UtcNow;

        // Data quando o gestor respondeu (Aceitar/Recusar)
        public DateTime? RespondidaEm { get; set; }

        [StringLength(1000)]
        public string? MotivoRejeicao { get; set; }

        [StringLength(1000)]
        public string? ObservacoesTutor { get; set; }

        // Check-in e Check-out efetivos
        public DateTime? DataCheckInEfetiva { get; set; }
        public TimeSpan? HoraCheckInEfetiva { get; set; }

        public DateTime? DataCheckOutEfetiva { get; set; }
        public TimeSpan? HoraCheckOutEfetiva { get; set; }

        [StringLength(1000)]
        public string? ObservacoeGestor { get; set; }

        // Componentes do valor calculado
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorDiariaBase { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorAdicionalIdade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }

        // Data da última atualização (para rastreamento)
        public DateTime? AtualizadaEm { get; set; }

        // Código de transação (para mock de pagamento)
        [StringLength(100)]
        public string? CodigoTransacao { get; set; }
    }
}
