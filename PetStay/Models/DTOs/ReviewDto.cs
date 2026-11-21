namespace PetStay.Models.DTOs
{
    /// <summary>
    /// DTO para exibir review/avaliação
    /// </summary>
    public class ReviewDto
    {
        public int Id { get; set; }

        public int HotelId { get; set; }

        public string? HotelNome { get; set; }

        public int Nota { get; set; }

        public string? TituloAvaliacao { get; set; }

        public string? Comentario { get; set; }

        public DateTime DataCriacao { get; set; }

        public string? TutorNome { get; set; }
    }

    /// <summary>
    /// DTO para listar reviews de um hotel
    /// </summary>
    public class HotelReviewsDto
    {
        public int HotelId { get; set; }

        public string? HotelNome { get; set; }

        public decimal NotaMedia { get; set; }

        public int QuantidadeAvaliacoes { get; set; }

        public List<ReviewDto>? Reviews { get; set; }
    }
}
