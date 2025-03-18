using TDT4900_MasterThesis.Engine;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Service;

public class SimulationBatchService(SimulationBatchEngine simulationBatchEngine)
{
    public async Task RunSimulationBatchAsync(
        int batchSize,
        bool persist,
        GraphSpec graphSpec,
        AlgorithmSpec algorithmSpec,
        CancellationToken cancellationToken
    )
    {
        var simulationBatch = new SimulationBatch()
        {
            PersistSimulations = persist,
            AlgorithmSpec = algorithmSpec,
            GraphSpec = graphSpec,
            Simulations = Enumerable.Range(0, batchSize).Select(_ => new Simulation()).ToList(),
        };

        await simulationBatchEngine.RunSimulationBatchAsync(simulationBatch, cancellationToken);
    }
}
