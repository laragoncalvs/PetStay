using PetStay.Models;

namespace PetStay.Models
{
    public class HotelSearchViewModel
    {
        public string? Cidade { get; set; }

        public string? Estado { get; set; }

        public DateTime? DataEntrada { get; set; }

        public DateTime? DataSaida { get; set; }

        public int? PetId { get; set; }

        public Pet? PetSelecionado { get; set; }

        public List<Pet>? PetsDoTutor { get; set; } = new();

        public List<HotelSearchResultViewModel>? Resultados { get; set; } = new();

        public bool FoiBuscado { get; set; }

        public string? MensagemErro { get; set; }

        // Campos de filtro
        public string? Raca { get; set; }

        public int? Tamanho { get; set; } // 0=Pequeno, 1=Médio, 2=Grande

        public int? FaixaEtaria { get; set; } // 0=Filhote, 1=Adulto, 2=Idoso

        public decimal? ValorMinimo { get; set; }

        public decimal? ValorMaximo { get; set; }

        public string? Cuidados { get; set; }

        // Campo de ordenação (avaliacao, valor, distancia)
        public string SortBy { get; set; } = "avaliacao";
    }

    public class HotelSearchResultViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Endereco { get; set; } = string.Empty;

        public string Cidade { get; set; } = string.Empty;

        public string? Estado { get; set; }

        public string? Telefone { get; set; }

        public string? Email { get; set; }

        public string? Descricao { get; set; }

        public decimal Avaliacao { get; set; }

        public string? ImagemUrl { get; set; }

        public decimal Diaria { get; set; }

        public int Capacidade { get; set; }

        public int VagasDisponiveis { get; set; }

        public TimeSpan HorarioAbertura { get; set; }

        public TimeSpan HorarioFechamento { get; set; }

        public string PortesAceitos { get; set; } = string.Empty;

        public string FaixasEtariasAceitas { get; set; } = string.Empty;

        public decimal ValorTotalEstimado { get; set; }

        public int DiasReserva { get; set; }
    }
}
