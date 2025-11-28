using fs_2025_assessment_1_75026.Models;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;


namespace fs_2025_assessment_1_75026.Services


{
    public class StationService : IStationService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<StationService> _logger;
        private const string CacheKey = "stations_v1";
        private readonly string _jsonFilePath;

        public MemoryCache Memory { get; }

        public StationService(IMemoryCache cache, ILogger<StationService> logger, IConfiguration config)
        {
            _cache = cache;
            _logger = logger;

            _jsonFilePath = config["DataFiles:Stations"]
                ?? "Data/stations.json";

            LoadStationsIntoCache();
        }

        public StationService(MemoryCache memory)
        {
            Memory = memory;
        }

        private void LoadStationsIntoCache()
        {
            if (!File.Exists(_jsonFilePath))
            {
                _logger.LogError("Stations JSON file missing: {path}", _jsonFilePath);
                _cache.Set(CacheKey, new List<Station>());
                return;
            }

            var json = File.ReadAllText(_jsonFilePath);
            var stations = JsonSerializer.Deserialize<List<Station>>(json)
                           ?? new List<Station>();

            _logger.LogInformation("Loaded {count} stations into cache.", stations.Count);

            _cache.Set(CacheKey, stations);
        }

        public List<Station> GetAllStations()
        {
            return _cache.Get<List<Station>>(CacheKey) ?? new List<Station>();
        }

        public Station? GetStationByNumber(int number)
        {
            return GetAllStations().FirstOrDefault(s => s.Number == number);
        }

        public void SaveAllStations(List<Station> stations)
        {
            _cache.Set(CacheKey, stations);
        }
    }
}