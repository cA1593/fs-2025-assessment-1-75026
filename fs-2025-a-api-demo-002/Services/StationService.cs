using fs_2025_assessment_1_75026.Models;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;


namespace fs_2025_assessment_1_75026.Services

// Service interface
{
    public interface IStationService
    {
        List<Station> GetAllStations();
        Station? GetStationByNumber(int number);
    }

    // Implementation of the StationService
    public class StationService : IStationService
    {
        private readonly List<Station> _stations;
        private readonly IMemoryCache _cache;
        private const string ALL_STATIONS_CACHE_KEY = "all_stations";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        // Constructor
        public StationService(IMemoryCache cache)
        {
            _cache = cache;
            _stations = LoadStationsFromJson();
        }

        // Load stations from JSON file
        private List<Station> LoadStationsFromJson()
        {
            try
            {
                string filePath = Path.Combine(AppContext.BaseDirectory, "Data", "dublinbike.json");

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Station data file not found at: {filePath}");
                }

                string jsonContent = File.ReadAllText(filePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                  
                };

                var stations = JsonSerializer.Deserialize<List<Station>>(jsonContent, options);


                Console.WriteLine($"Loaded {stations?.Count ?? 0} stations from JSON");

                return stations ?? new List<Station>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stations: {ex.Message}");
                return new List<Station>();
            }
        }


        // Get all stations with caching
        public List<Station> GetAllStations()
        {
            return _cache.GetOrCreate(ALL_STATIONS_CACHE_KEY, entry =>

            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                Console.WriteLine($"Cache miss - Loading stations into cache (expires in {CacheDuration.TotalMinutes} minutes)");
                return _stations;

            }) ?? new List<Station>();
        }


        // Get a station by its number with caching
        public Station? GetStationByNumber(int number)
        {

            string cacheKey = $"station_{number}";

            return _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                var station = _stations.FirstOrDefault(s => s.Number == number);

                if (station != null)
                {
                    Console.WriteLine($"Cache miss - Loading station {number} into cache");
                }

                return station;
            });
        }
    }
}