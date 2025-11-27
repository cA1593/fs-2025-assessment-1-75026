using fs_2025_assessment_1_75026.Models;
using fs_2025_assessment_1_75026.Services;
using Microsoft.AspNetCore.Mvc;


namespace fs_2025_assessment_1_75026.Endpoints
{
    public static class StationEndPoints
    {
        public static void AddStationEndPoints(this WebApplication app)
        {
            var stationGroup = app.MapGroup("/api/v1/stations")
                .WithTags("Stations V1")
                .WithOpenApi();

            // GET /api/v1/stations
            stationGroup.MapGet("/", GetAllStations)
                .WithName("GetAllStationsV1")
                .WithSummary("Get all bike stations with filtering, searching, sorting, and paging");

            // GET /api/v1/stations/{number}
            stationGroup.MapGet("/{number}", GetStationByNumber)
                .WithName("GetStationByNumberV1")
                .WithSummary("Get a specific station by its number");

            // GET /api/v1/stations/summary
            stationGroup.MapGet("/summary", GetStationsSummary)
                .WithName("GetStationsSummaryV1")
                .WithSummary("Get summary statistics for all stations");

            // POST /api/v1/stations
            stationGroup.MapPost("/", CreateStation)
                .WithName("CreateStationV1")
                .WithSummary("Create a new station");

            // PUT /api/v1/stations/{number}
            stationGroup.MapPut("/{number}", UpdateStation)
                .WithName("UpdateStationV1")
                .WithSummary("Update an existing station");
        }
        // GET /api/v1/stations
        private static IResult GetAllStations(
            IStationService stationService,
            [FromQuery] string? status,
            [FromQuery] int? minBikes,
            [FromQuery] string? q,
            [FromQuery] string? sort,
            [FromQuery] string? dir,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var stations = stationService.GetAllStations();

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                stations = stations.Where(s =>
                    s.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filter by minimum bikes
            if (minBikes.HasValue)
            {
                stations = stations.Where(s => s.AvailableBikes >= minBikes.Value).ToList();
            }

            // Search by name or address
            if (!string.IsNullOrEmpty(q))
            {
                stations = stations.Where(s =>
                    s.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    s.Address.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Sort
            var sortField = sort?.ToLower() ?? "name";
            var direction = dir?.ToLower() ?? "asc";

            stations = sortField switch
            {
                "availablebikes" => direction == "desc"
                    ? stations.OrderByDescending(s => s.AvailableBikes).ToList()
                    : stations.OrderBy(s => s.AvailableBikes).ToList(),
                "occupancy" => direction == "desc"
                    ? stations.OrderByDescending(s => s.Occupancy).ToList()
                    : stations.OrderBy(s => s.Occupancy).ToList(),
                _ => direction == "desc"
                    ? stations.OrderByDescending(s => s.Name).ToList()
                    : stations.OrderBy(s => s.Name).ToList()
            };

            // Pagination
            var totalStations = stations.Count;
            var totalPages = (int)Math.Ceiling(totalStations / (double)pageSize);

            var paginatedStations = stations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new
            {
                page,
                pageSize,
                totalStations,
                totalPages,
                data = paginatedStations
            };

            return Results.Ok(result);
        }

        // GET /api/v1/stations/{number}
        private static IResult GetStationByNumber(
            IStationService stationService,
            int number)
        {
            var station = stationService.GetStationByNumber(number);

            if (station == null)
            {
                return Results.NotFound(new { message = $"Station with number {number} not found" });
            }

            return Results.Ok(station);
        }

        // GET /api/v1/stations/summary
        private static IResult GetStationsSummary(IStationService stationService)
        {
            var stations = stationService.GetAllStations();

            var summary = new
            {
                totalStations = stations.Count,
                totalBikeStands = stations.Sum(s => s.BikeStands),
                totalAvailableBikes = stations.Sum(s => s.AvailableBikes),
                totalAvailableStands = stations.Sum(s => s.AvailableBikeStands),
                statusCounts = stations.GroupBy(s => s.Status)
                    .Select(g => new { status = g.Key, count = g.Count() })
                    .ToList()
            };

            return Results.Ok(summary);
        }

        // POST /api/v1/stations
        private static IResult CreateStation(
            IStationService stationService,
            [FromBody] Station newStation)
        {
            // For now, just return the created station
            // In a real app, you'd add it to the service
            return Results.Created($"/api/v1/stations/{newStation.Number}", newStation);
        }

        // PUT /api/v1/stations/{number}
        private static IResult UpdateStation(
            IStationService stationService,
            int number,
            [FromBody] Station updatedStation)
        {
            var existingStation = stationService.GetStationByNumber(number);

            if (existingStation == null)
            {
                return Results.NotFound(new { message = $"Station with number {number} not found" });
            }

            // For now, just return the updated station
            // In a real app, you'd update it in the service
            return Results.Ok(updatedStation);
        }
    }
}