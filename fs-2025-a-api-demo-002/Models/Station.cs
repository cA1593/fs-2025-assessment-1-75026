using Microsoft.Azure.Cosmos.Spatial;
using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace fs_2025_assessment_1_75026.Models
{
    public class Station
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("contractName")]
        public string ContractName { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("address")]
        public string Address { get; set; } = string.Empty;

        [JsonProperty("position")]
        public Position Position { get; set; } = new Position();

        [JsonProperty("banking")]
        public bool Banking { get; set; }

        [JsonProperty("bonus")]
        public bool Bonus { get; set; }

        [JsonProperty("bike_stands")]
        public int BikeStands { get; set; }

        [JsonProperty("available_bike_stands")]
        public int AvailableBikeStands { get; set; }

        [JsonProperty("available_bikes")]
        public int AvailableBikes { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("last_update")]
        public long LastUpdate { get; set; }

        // Optional: Computed property, not saved in DB
        [System.Text.Json.Serialization.JsonIgnore]
        public double Occupancy =>
            BikeStands == 0 ? 0 : (double)AvailableBikes / BikeStands;
    }

    public class Position
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }
}