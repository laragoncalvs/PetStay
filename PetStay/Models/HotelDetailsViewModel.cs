namespace PetStay.Models
{
    public class HotelDetailsViewModel
    {
        public Hotel Hotel { get; set; } = new();

        public string? Cidade { get; set; }

        public string? Estado { get; set; }

        public DateTime? DataEntrada { get; set; }

        public DateTime? DataSaida { get; set; }

        public int? PetId { get; set; }

        public List<Pet> PetsDoTutor { get; set; } = new();
    }
}
