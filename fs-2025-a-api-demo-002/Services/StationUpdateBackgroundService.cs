using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace fs_2025_assessment_1_75026.Services
{

    // Background service to update station data periodically
    public class StationUpdateBackgroundService : BackgroundService
    {
        private readonly ILogger<StationUpdateBackgroundService> _logger;
        private readonly IStationService _stationService;
        private readonly Random _random = new();

        public StationUpdateBackgroundService(
            ILogger<StationUpdateBackgroundService> logger,
            IStationService stationService)
        {
            _logger = logger;
            _stationService = stationService;
        }


        // Periodically update station data
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Station background updater started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                UpdateStations();

                // Wait 15 seconds (can be 10–20 seconds per assignment requirement)
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }


        // Update station availability with random data
        private void UpdateStations()
        {
            var stations = _stationService.GetAllStations();

            foreach (var station in stations)
            {
                // Generate new random availability
                int newAvailableBikes = _random.Next(0, station.BikeStands + 1);
                int newAvailableStands = station.BikeStands - newAvailableBikes;

                station.AvailableBikes = newAvailableBikes;
                station.AvailableBikeStands = newAvailableStands;
                station.LastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            _logger.LogInformation("🔄 Updated {count} stations at {time}",
                stations.Count,
                DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}