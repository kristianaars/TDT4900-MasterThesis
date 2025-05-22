using System.Text.Json;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Repository;

public class EventHistoryRepository : IEventHistoryRepository
{
    public async Task PersistNodeEventListAsync(
        int simulationId,
        List<NodeEvent>? nodeEvents,
        CancellationToken? cancellationToken = null
    )
    {
        string fileName =
            $"/Users/kristianaars/RiderProjects/TDT4900-MasterThesis/TDT4900-MasterThesis/output/{simulationId}.json";

        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // optional, for better readability
        };

        await using var stream = File.Create(fileName);

        await JsonSerializer.SerializeAsync(
            stream,
            nodeEvents,
            options,
            cancellationToken ?? CancellationToken.None
        );
    }
}

public interface IEventHistoryRepository
{
    public Task PersistNodeEventListAsync(
        int simulationId,
        List<NodeEvent> nodeEvents,
        CancellationToken? cancellationToken = null
    );
}
