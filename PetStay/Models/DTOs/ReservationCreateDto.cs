namespace PetStay.Models.DTOs
{
    /// <summary>
    /// DTO para resposta GET Create - lista de pets do tutor
    /// </summary>
    public class ReservationCreateResponseDto
    {
        public int HotelId { get; set; }

        public string? HotelNome { get; set; }

        public decimal ValorDiariaBase { get; set; }

        public List<PetResumoDto>? PetsDoTutor { get; set; }

        public DateTime DataEntrada { get; set; }

        public DateTime DataSaida { get; set; }

        public TimeSpan HorarioEntrada { get; set; }

        public TimeSpan HorarioSaida { get; set; }

        public bool VagasDisponiveis { get; set; }

        public bool PetCompativel { get; set; }
    }

    /// <summary>
    /// DTO resumido para pet
    /// </summary>
    public class PetResumoDto
    {
        public int Id { get; set; }

        public string? Nome { get; set; }

        public string? Raca { get; set; }

        public DateTime DataNascimento { get; set; }
    }
}
