namespace PetStay.Models.DTOs
{
    /// <summary>
    /// DTO para exibir detalhes da reserva sem ciclos de referência
    /// </summary>
    public class ReservationDetailDto
    {
        public int Id { get; set; }

        public int PetId { get; set; }

        public string? PetNome { get; set; }

        public int HotelId { get; set; }

        public string? HotelNome { get; set; }

        public string? HotelEndereco { get; set; }

        public string? HotelCidade { get; set; }

        public string? HotelEstado { get; set; }

        public string? HotelTelefone { get; set; }

        public string? HotelEmail { get; set; }

        public DateTime DataEntrada { get; set; }

        public DateTime DataSaida { get; set; }

        public TimeSpan? HorarioEntrada { get; set; }

        public TimeSpan? HorarioSaida { get; set; }

        public string? Status { get; set; }

        public decimal ValorDiariaBase { get; set; }

        public decimal ValorAdicionalIdade { get; set; }

        public decimal ValorTotal { get; set; }

        public string? ObservacoesTutor { get; set; }

        public DateTime CriadaEm { get; set; }

        public DateTime? AtualizadaEm { get; set; }

        public string? CodigoTransacao { get; set; }
    }

    /// <summary>
    /// DTO para listar reservas (resumo)
    /// </summary>
    public class ReservationListDto
    {
        public int Id { get; set; }

        public string? HotelNome { get; set; }

        public string? PetNome { get; set; }

        public DateTime DataEntrada { get; set; }

        public DateTime DataSaida { get; set; }

        public string? Status { get; set; }

        public decimal ValorTotal { get; set; }

        public DateTime CriadaEm { get; set; }
    }
}
