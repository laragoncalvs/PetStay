using Microsoft.EntityFrameworkCore;   // <- necessário
using PetStay.Models;

namespace PetStay.Data
{
    public class AppDbContext : DbContext   // <- necessário
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }
    }
}