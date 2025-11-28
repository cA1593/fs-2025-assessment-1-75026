using fs_2025_assessment_1_75026.Models;
using Microsoft.Azure.Cosmos;

public class CosmosStationService
{
    private readonly Container _container;

    public CosmosStationService(CosmosClient client, IConfiguration config)
    {
        var dbName = config["CosmosDb:DatabaseName"];
        var containerName = config["CosmosDb:ContainerName"];

        _container = client.GetContainer(dbName, containerName);
    }

    // Load all stations (small dataset, fine to do)
    public async Task<List<Station>> GetAllStationsAsync()
    {
        var query = _container.GetItemQueryIterator<Station>(
            new QueryDefinition("SELECT * FROM c"),
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = null  // CROSS PARTITION QUERY
            });

        List<Station> results = new();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    // Get station by number (reliable)
    public async Task<Station?> GetStationByNumberAsync(int number)
    {
        var stations = await GetAllStationsAsync();
        return stations.FirstOrDefault(s => s.Number == number);
    }

    // CREATE — do NOT modify partition key case
    public async Task<Station> CreateStationAsync(Station station)
    {
        if (string.IsNullOrWhiteSpace(station.Id))
            station.Id = Guid.NewGuid().ToString();

        var response = await _container.CreateItemAsync(
            station,
            new PartitionKey(station.ContractName)  // EXACT partition key
        );

        return response.Resource;
    }

    // UPDATE — do NOT modify partition key case
    public async Task<Station> UpdateStationAsync(Station station)
    {
        var response = await _container.UpsertItemAsync(
            station,
            new PartitionKey(station.ContractName) // EXACT partition key
        );

        return response.Resource;
    }
}