using Xunit;
using fs_2025_assessment_1_75026.Services;
using fs_2025_assessment_1_75026.Models;
using Microsoft.Extensions.Caching.Memory;

namespace fs_2025_assessment_1_75026.Tests
{
    public class StationFilterTests
    {
        private StationService CreateServiceWithTestData()
        {
            var memory = new MemoryCache(new MemoryCacheOptions());

            var service = new StationService(memory);

            service.GetAllStations().Clear();
            service.GetAllStations().AddRange(new List<Station>
            {
                new Station { Number = 1, Status = "OPEN", Name = "A", AvailableBikes = 5, BikeStands = 10 },
                new Station { Number = 2, Status = "CLOSED", Name = "B", AvailableBikes = 2, BikeStands = 10 },
                new Station { Number = 3, Status = "OPEN", Name = "C", AvailableBikes = 8, BikeStands = 12 }
            });

            return service;
        }

        [Fact]
        public void Filter_Open_Stations_Should_Return_Expected_Count()
        {
            var service = CreateServiceWithTestData();

            var openStations = service.GetAllStations()
                                      .Where(s => s.Status == "OPEN")
                                      .ToList();

            Assert.Equal(2, openStations.Count);
        }
    }
}