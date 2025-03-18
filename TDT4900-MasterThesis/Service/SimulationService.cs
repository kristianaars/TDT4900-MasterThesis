using TDT4900_MasterThesis.Algorithm;
using TDT4900_MasterThesis.Engine;
using TDT4900_MasterThesis.Factory;
using TDT4900_MasterThesis.Model;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Service;

public class SimulationService(
    SimulationEngine simulationEngine,
    SimulationJobFactory simulationJobFactory
)
{
    /// <summary>
    /// Run a simulation job in addition to verification of the results.
    /// This will first run a Dijkstras search to find the actual shortest path, then run the simulation job.
    /// Both results will be saved in the Simulation object.
    /// </summary>
    /// <param name="simulation">Simulation to run</param>
    /// <param name="algorithmSpec">Algorithm specification to use in the simulation job</param>
    /// <param name="cancellationToken">Cancellation token to stop the simulation</param>
    public async Task RunSimulationWithVerificationAsync(
        Simulation simulation,
        AlgorithmSpec algorithmSpec,
        CancellationToken cancellationToken
    )
    {
        // Build the simulation jobs to run
        var dijkstraSimulationJob = simulationJobFactory.GetSimulationJob(
            simulation,
            new AlgorithmSpec { AlgorithmType = AlgorithmType.Dijkstras }
        );
        var simulationJob = simulationJobFactory.GetSimulationJob(simulation, algorithmSpec);

        // Run Dijkstras search to find the actual shortest path (and verify that such path exists)
        await simulationEngine.RunSimulationJobAsync(dijkstraSimulationJob, cancellationToken);
        var dijkstraResult = dijkstraSimulationJob.Algorithm.CalculateResult();

        // Run the actual algorithm, and calculate the result
        await simulationEngine.RunSimulationJobAsync(simulationJob, cancellationToken);
        var algorithmResult = simulationJob.Algorithm.CalculateResult();

        // Save the results to the Simulation object
        simulation.ShortestPathLength = dijkstraResult.Distance;
        simulation.AlgorithmShortestPathLength = algorithmResult.Distance;

        // Persist the tagged nodes in the Simulation object
        simulation.ShortestPathTaggedNodes = dijkstraResult
            .GraphTagged.Nodes.Select(n => simulation.Graph!.Nodes[n.NodeId])
            .ToList();

        simulation.AlgorithmTaggedNodes = algorithmResult
            .GraphTagged.Nodes.Select(n => simulation.Graph!.Nodes[n.NodeId])
            .ToList();

        // Add EventHistory to the Simulation object
        simulation.EventHistory = simulationJob.Algorithm.EventHistory;
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
