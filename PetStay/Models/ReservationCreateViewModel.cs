using PetStay.Models;
using System.ComponentModel.DataAnnotations;

namespace PetStay.Models
{
    public class ReservationCreateViewModel
    {
        public int HotelId { get; set; }

        public Hotel? Hotel { get; set; }

        [Required(ErrorMessage = "Selecione um pet para a reserva.")]
        public int PetId { get; set; }

        public Pet? PetSelecionado { get; set; }

        public List<Pet>? PetsDoTutor { get; set; }

        [Required(ErrorMessage = "Data de entrada é obrigatória.")]
        public DateTime DataEntrada { get; set; }

        [Required(ErrorMessage = "Data de saída é obrigatória.")]
        public DateTime DataSaida { get; set; }

        public TimeSpan HorarioEntrada { get; set; } = new TimeSpan(14, 0, 0); // 14:00 por padrão

        public TimeSpan HorarioSaida { get; set; } = new TimeSpan(11, 0, 0); // 11:00 por padrão

        [StringLength(1000, ErrorMessage = "As observações não podem ter mais de 1000 caracteres.")]
        public string? ObservacoesTutor { get; set; }

        // Para exibição
        public int DiasReserva => (DataSaida.Date - DataEntrada.Date).Days;

        public decimal ValorDiariaBase { get; set; }

        public decimal ValorAdicionalIdade { get; set; }

        public decimal ValorTotal { get; set; }

        // Resumo para confirmação
        public string? ResumoDisponibilidade { get; set; }

        public bool VagasDisponiveis { get; set; }

        public bool PetCompativel { get; set; }

        public string? MensagemValidacao { get; set; }
    }
}
