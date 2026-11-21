namespace PetStay.Models
{
    public class ReservationConfirmViewModel
    {
        public int ReservationId { get; set; }

        public Hotel? Hotel { get; set; }

        public Pet? PetSelecionado { get; set; }

        public string? HotelNome { get; set; }

        public string? PetNome { get; set; }

        public DateTime DataEntrada { get; set; }

        public DateTime DataSaida { get; set; }

        public TimeSpan HorarioEntrada { get; set; }

        public TimeSpan HorarioSaida { get; set; }

        public int DiasReserva { get; set; }

        public decimal ValorDiariaBase { get; set; }

        public decimal ValorAdicionalIdade { get; set; }

        public decimal ValorTotal { get; set; }

        public string? ObservacoesTutor { get; set; }

        // Informações de pagamento
        public string? MetodoPagamento { get; set; }

        public string? UltimosDigitos { get; set; }

        public string? Status { get; set; }
    }
}
