using System;
using System.Collections.Generic;

namespace PetStay.Models
{
    public class ReservationReportViewModel
    {
        // Período do relatório
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        // Resumo de métricas principais (cards)
        public decimal TotalFaturado { get; set; }
        public decimal TaxaOcupacao { get; set; }
        public int ReservasConfirmadas { get; set; }
        public decimal OcupacaoMedia { get; set; }

        // Dados por status
        public int TotalPendentes { get; set; }
        public int TotalConfirmadas { get; set; }
        public int TotalEmProgresso { get; set; }
        public int TotalFinalizadas { get; set; }
        public int TotalCanceladas { get; set; }

        // Dados de faturamento diário para gráfico de linha
        public List<DadoFaturamentoDiario> FaturaimentoPorDia { get; set; } = new List<DadoFaturamentoDiario>();

        // Dados de ocupação por dia
        public List<DadoOcupacaoDia> OcupacaoPorDia { get; set; } = new List<DadoOcupacaoDia>();

        // Lista detalhada de reservas
        public List<ReservationDetalheDto> Reservas { get; set; } = new List<ReservationDetalheDto>();

        // Informações do hotel
        public int HotelId { get; set; }
        public string NomeHotel { get; set; } = string.Empty;
        public int CapacidadeHotel { get; set; }
    }

    public class DadoFaturamentoDiario
    {
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public int QuantidadeReservas { get; set; }
    }

    public class DadoOcupacaoDia
    {
        public DateTime Data { get; set; }
        public decimal Ocupacao { get; set; } // Percentual 0-100
        public int HospedesOcupando { get; set; }
    }

    public class ReservationDetalheDto
    {
        public int Id { get; set; }
        public string PetNome { get; set; } = string.Empty;
        public string TutorNome { get; set; } = string.Empty;
        public string HotelNome { get; set; } = string.Empty;
        public DateTime DataEntrada { get; set; }
        public DateTime DataSaida { get; set; }
        public StatusReserva Status { get; set; }
        public decimal ValorTotal { get; set; }
        public int DiariasReservadas { get; set; }
    }
}
