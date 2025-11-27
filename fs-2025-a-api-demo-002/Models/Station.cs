using System.Text.Json.Serialization;

namespace fs_2025_assessment_1_75026.Models
{
    public class Station
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("contract_name")]
        public string ContractName { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("position")]
        public GeoPosition Position { get; set; } = new();

        [JsonPropertyName("banking")]
        public bool Banking { get; set; }

        [JsonPropertyName("bonus")]
        public bool Bonus { get; set; }

        [JsonPropertyName("bike_stands")]
        public int BikeStands { get; set; }

        [JsonPropertyName("available_bike_stands")]
        public int AvailableBikeStands { get; set; }

        [JsonPropertyName("available_bikes")]
        public int AvailableBikes { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("last_update")]
        public long LastUpdate { get; set; }

        // Computed properties 
        public DateTime LastUpdateDateTime =>
            DateTimeOffset.FromUnixTimeMilliseconds(LastUpdate).DateTime;

        public DateTime LastUpdateLocal =>
            TimeZoneInfo.ConvertTimeFromUtc(
                LastUpdateDateTime,
                TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")
            );

        public double Occupancy =>
            BikeStands > 0 ? (double)AvailableBikes / BikeStands : 0.0;
    }

    public class GeoPosition
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lng")]
        public double Lng { get; set; }
    }
}