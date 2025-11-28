using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace fs_2025_assessment_1_75026.Tests
{
    public class StationEndpointTests :
        IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public StationEndpointTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllStations_Returns_OK()
        {
            var response = await _client.GetAsync("/api/v1/stations");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}