using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStay.Data;
using PetStay.Models;
using PetStay.Models.DTOs;

namespace PetStay.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Review/Create?reservationId=5
        [HttpGet("create")]
        public async Task<IActionResult> Create(int reservationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var reservation = await _context.Reservations
                .Include(r => r.Hotel)
                .Include(r => r.Pet)
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.TutorId == user.Id);

            if (reservation == null)
                return NotFound();


            // Verificar se já existe review
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (existingReview != null)
                return BadRequest(new { message = "Esta reserva já foi avaliada." });

            var viewModel = new ReviewViewModel
            {
                ReservationId = reservationId,
                HotelId = reservation.HotelId,
                HotelNome = reservation.Hotel?.Nome,
                PetNome = reservation.Pet?.Nome,
                DataEntrada = reservation.DataEntrada,
                DataSaida = reservation.DataSaida,
                ValorTotal = reservation.ValorTotal,
                Nota = 5 // Padrão 5 estrelas
            };

            return Ok(viewModel);
        }

        // POST: Review/Create
       
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ReviewViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == model.ReservationId && r.TutorId == user.Id);
            if (reservation == null)
                return NotFound();
            if (reservation.Status != StatusReserva.Finalizada)
                return BadRequest(new { message = "Somente reservas finalizadas podem ser avaliadas." });
            var existingReview = await _context.Reviews
                .AnyAsync(r => r.ReservationId == model.ReservationId);
            if (existingReview)
                return BadRequest(new { message = "Esta reserva já foi avaliada." });
            if (model.Nota < 1 || model.Nota > 5)
                return BadRequest(new { message = "A nota deve estar entre 1 e 5 estrelas." });
            var review = new Review
            {
                HotelId = reservation.HotelId,
                TutorId = user.Id,
                ReservationId = reservation.Id,
                Nota = model.Nota,
                TituloAvaliacao = model.TituloAvaliacao,
                Comentario = model.Comentario,
                DataCriacao = DateTime.UtcNow
            };
            
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                await AtualizaNoteaHotel(reservation.HotelId);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            });
            return Ok(new
            {
                message = "Avaliação registrada com sucesso!",
                reviewId = review.Id
            });
        }


        // GET: Review/Hotel/{hotelId}
        [AllowAnonymous]
        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> HotelReviews(int hotelId)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null)
                return NotFound();

            var reviews = await _context.Reviews
                .Where(r => r.HotelId == hotelId)
                .Include(r => r.Tutor)
                .OrderByDescending(r => r.DataCriacao)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    HotelId = r.HotelId,
                    HotelNome = hotel.Nome,
                    Nota = r.Nota,
                    TituloAvaliacao = r.TituloAvaliacao,
                    Comentario = r.Comentario,
                    DataCriacao = r.DataCriacao,
                    TutorNome = r.Tutor != null ? r.Tutor.UserName : "Anônimo"
                })
                .ToListAsync();

            var dto = new HotelReviewsDto
            {
                HotelId = hotelId,
                HotelNome = hotel.Nome,
                NotaMedia = hotel.Avaliacao,
                QuantidadeAvaliacoes = reviews.Count,
                Reviews = reviews
            };

            return Ok(dto);
        }

        // GET: Review/MyReviews
        [HttpGet("my-reviews")]
        public async Task<IActionResult> MyReviews()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var reviews = await _context.Reviews
                .Where(r => r.TutorId == user.Id)
                .Include(r => r.Hotel)
                .OrderByDescending(r => r.DataCriacao)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    HotelId = r.HotelId,
                    HotelNome = r.Hotel!.Nome,
                    Nota = r.Nota,
                    TituloAvaliacao = r.TituloAvaliacao,
                    Comentario = r.Comentario,
                    DataCriacao = r.DataCriacao,
                    TutorNome = user.UserName
                })
                .ToListAsync();

            return Ok(reviews);
        }

        // DELETE: Review/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            // Apenas o autor ou um admin pode deletar
            if (review.TutorId != user.Id)
                return Forbid();

            var hotelId = review.HotelId;

            _context.Reviews.Remove(review);

            // Atualizar a nota média do hotel
            await AtualizaNoteaHotel(hotelId);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Avaliação deletada com sucesso." });
        }

        // Métodos auxiliares
        private async Task AtualizaNoteaHotel(int hotelId)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel == null)
                return;

            var reviews = await _context.Reviews
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();

            if (reviews.Count == 0)
                hotel.Avaliacao = 0;
            else
                hotel.Avaliacao = (decimal)reviews.Average(r => r.Nota);

            _context.Hotels.Update(hotel);
        }
    }
}
