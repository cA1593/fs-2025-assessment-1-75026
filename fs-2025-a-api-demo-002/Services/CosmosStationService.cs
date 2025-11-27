using fs_2025_assessment_1_75026.Models;
using Microsoft.Azure.Cosmos;
using System.ComponentModel;

namespace fs_2025_assessment_1_75026.Services
{

    
    public class CosmosStationService
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;


        public CosmosStationService(CosmosClient client, IConfiguration config)
        {
            var dbName = config["CosmosDb:DatabaseName"];
            var containerName = config["CosmosDb:ContainerName"];

            _container = client.GetContainer(dbName, containerName);
        }

        public async Task<List<Station>> GetAllStationsAsync()
        {
            var query = _container.GetItemQueryIterator<Station>(
                new QueryDefinition("SELECT * FROM c")
            );

            List<Station> results = new();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<Station?> GetStationByNumberAsync(int number)
        {
            var query = _container.GetItemQueryIterator<Station>(
                new QueryDefinition("SELECT * FROM c WHERE c.number = @num")
                    .WithParameter("@num", number)
            );

            if (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                return response.FirstOrDefault();
            }

            return null;
        }
    }
}