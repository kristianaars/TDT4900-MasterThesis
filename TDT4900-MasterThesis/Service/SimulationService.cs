using TDT4900_MasterThesis.Engine;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Service;

public class SimulationService(
    SimulationBatchEngine simulationBatchEngine,
    SimulationEngine simulationEngine,
    SimulationPersistenceService persistenceService
)
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
            Simulations = Enumerable
                .Range(0, batchSize)
                .Select(_ => new Simulation
                {
                    GraphSpec = graphSpec,
                    AlgorithmSpec = algorithmSpec,
                })
                .ToList(),
        };

        await simulationBatchEngine.RunSimulationBatchAsync(simulationBatch, cancellationToken);
    }

    public void PauseSimulation()
    {
        simulationEngine.Pause();
    }

    public void ResumeSimulation()
    {
        simulationEngine.Resume();
    }

    public void SetTargetFps(int targetFps)
    {
        simulationEngine.TargetFps = targetFps;
    }

    public void SetTargetTps(int targetTps)
    {
        simulationEngine.TargetTps = targetTps;
    }
}
