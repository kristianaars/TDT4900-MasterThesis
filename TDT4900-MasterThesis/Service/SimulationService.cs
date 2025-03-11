using TDT4900_MasterThesis.Engine;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Service;

public class SimulationService(
    SimulationBatchEngine simulationBatchEngine,
    SimulationEngine simulationEngine
)
{
    public async Task RunSimulationBatchAsync(
        SimulationBatch simulationBatch,
        CancellationToken cancellationToken
    )
    {
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
