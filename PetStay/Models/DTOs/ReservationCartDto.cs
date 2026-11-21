namespace PetStay.Models.DTOs
{
    public class ReservationCartDto
    {
        public int HotelId { get; set; }
        public int PetId { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime DataSaida { get; set; }
        public TimeSpan HorarioEntrada { get; set; }
        public TimeSpan HorarioSaida { get; set; }
        public string? ObservacoesTutor { get; set; }
        public decimal ValorDiariaBase { get; set; }
        public decimal ValorAdicionalIdade { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
