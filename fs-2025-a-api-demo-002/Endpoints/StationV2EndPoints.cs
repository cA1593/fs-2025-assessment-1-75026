using fs_2025_assessment_1_75026.Models;
using fs_2025_assessment_1_75026.Services;
using Microsoft.AspNetCore.Mvc;

namespace fs_2025_assessment_1_75026.Endpoints
{
    public static class StationV2EndPoints
    {
        public static void AddStationV2EndPoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/v2/stations")
                           .WithTags("Stations V2 (CosmosDB)")
                           .WithOpenApi();

            // GET /api/v2/stations
            
            group.MapGet("/", async (
                CosmosStationService cosmosService,
                [FromQuery] string? status,
                [FromQuery] int? minBikes,
                [FromQuery] string? q,
                [FromQuery] string? sort,
                [FromQuery] string? dir,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10
            ) =>
            {
                var stations = await cosmosService.GetAllStationsAsync();

                // Filtering logic (same as V1)
                if (!string.IsNullOrEmpty(status))
                    stations = stations.Where(s => s.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();

                if (minBikes.HasValue)
                    stations = stations.Where(s => s.AvailableBikes >= minBikes.Value).ToList();

                if (!string.IsNullOrEmpty(q))
                    stations = stations.Where(s =>
                        s.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                        s.Address.Contains(q, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                // Sorting
                sort = sort?.ToLower() ?? "name";
                dir = dir?.ToLower() ?? "asc";

                stations = sort switch
                {
                    "availablebikes" => dir == "desc"
                        ? stations.OrderByDescending(s => s.AvailableBikes).ToList()
                        : stations.OrderBy(s => s.AvailableBikes).ToList(),

                    "occupancy" => dir == "desc"
                        ? stations.OrderByDescending(s => s.Occupancy).ToList()
                        : stations.OrderBy(s => s.Occupancy).ToList(),

                    _ => dir == "desc"
                        ? stations.OrderByDescending(s => s.Name).ToList()
                        : stations.OrderBy(s => s.Name).ToList()
                };

                // Paging
                var total = stations.Count;
                var totalPages = (int)Math.Ceiling(total / (double)pageSize);

                var paginated = stations
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Results.Ok(new
                {
                    page,
                    pageSize,
                    totalStations = total,
                    totalPages,
                    data = paginated
                });
            });

            // GET /api/v2/stations/{number}
         
            group.MapGet("/{number}", async (CosmosStationService cosmosService, int number) =>
            {
                var station = await cosmosService.GetStationByNumberAsync(number);

                return station is null
                    ? Results.NotFound(new { message = $"Station {number} not found (CosmosDB)" })
                    : Results.Ok(station);
            });

            
            // GET /api/v2/stations/summary
            
            group.MapGet("/summary", async (CosmosStationService cosmosService) =>
            {
                var stations = await cosmosService.GetAllStationsAsync();

                var summary = new
                {
                    totalStations = stations.Count,
                    totalBikeStands = stations.Sum(s => s.BikeStands),
                    totalAvailableBikes = stations.Sum(s => s.AvailableBikes),
                    totalAvailableStands = stations.Sum(s => s.AvailableBikeStands),
                    statusCounts = stations
                        .GroupBy(s => s.Status)
                        .Select(g => new { status = g.Key, count = g.Count() })
                        .ToList()
                };

                return Results.Ok(summary);
            });

            
            // POST /api/v2/stations
            
            group.MapPost("/", async (
                CosmosStationService cosmosService,
                [FromBody] Station station) =>
            {
                var created = await cosmosService.CreateStationAsync(station);
                return Results.Created($"/api/v2/stations/{station.Number}", created);
            });

        }
    }
}