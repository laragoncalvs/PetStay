using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetStay.Models;

namespace PetStay.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Tarifa> Tarifas { get; set; }


        public DbSet<Pet> Pets { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.GestorDono)
                .WithMany(u => u.GestoresFuncionarios)
                .HasForeignKey(u => u.GestorDonoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tarifa>()
                .HasOne(t => t.Hotel)
                .WithMany(h => h.Tarifas)
                .HasForeignKey(t => t.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tarifa>()
                .HasIndex(t => new { t.HotelId, t.Porte })
                .IsUnique();

            modelBuilder.Entity<Hotel>()
                .HasOne(hotel => hotel.Gestor)
                .WithMany(user => user.Hoteis)
                .HasForeignKey(hotel => hotel.GestorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hotel>()
                .HasOne(hotel => hotel.Administrador)
                .WithMany(user => user.HoteisAnalisados)
                .HasForeignKey(hotel => hotel.AdministradorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(res => res.Pet)
                .WithMany()
                .HasForeignKey(res => res.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(res => res.Hotel)
                .WithMany()
                .HasForeignKey(res => res.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(res => res.Tutor)
                .WithMany()
                .HasForeignKey(res => res.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(rev => rev.Hotel)
                .WithMany()
                .HasForeignKey(rev => rev.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(rev => rev.Tutor)
                .WithMany()
                .HasForeignKey(rev => rev.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(rev => rev.Reservation)
                .WithMany()
                .HasForeignKey(rev => rev.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}