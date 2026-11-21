using Microsoft.EntityFrameworkCore;
using PetStay.Data;
using PetStay.Models;

namespace PetStay.Services
{
    public class AutoCancelReservationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AutoCancelReservationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public AutoCancelReservationService(IServiceProvider serviceProvider, ILogger<AutoCancelReservationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessExpiredReservationsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar cancelamento automático de reservas");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ProcessExpiredReservationsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Buscar reservas pendentes há mais de 24 horas
                var cutoffTime = DateTime.UtcNow.AddHours(-24);
                var expiredReservations = await context.Reservations
                    .Where(r => r.Status == StatusReserva.Pendente && r.SolicitadaEm < cutoffTime)
                    .ToListAsync();

                if (expiredReservations.Any())
                {
                    foreach (var reservation in expiredReservations)
                    {
                        reservation.Status = StatusReserva.Cancelada;
                        reservation.RespondidaEm = DateTime.UtcNow;
                        reservation.MotivoRejeicao = "Cancelado automaticamente após 24 horas sem resposta do gestor.";

                        context.Reservations.Update(reservation);
                    }

                    await context.SaveChangesAsync();
                    _logger.LogInformation($"Auto-canceladas {expiredReservations.Count} reservas expiradas.");
                }
            }
        }
    }
}
