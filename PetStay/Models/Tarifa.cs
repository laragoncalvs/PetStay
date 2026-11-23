using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStay.Models
{
    public class Tarifa
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;
        public TamanhoPet Porte { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorDiaria { get; set; }
    }
}
