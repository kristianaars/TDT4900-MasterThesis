using GLib;
using TDT4900_MasterThesis.Algorithm;
using TDT4900_MasterThesis.Engine;
using TDT4900_MasterThesis.Factory;
using TDT4900_MasterThesis.Factory.SimulationJob;
using TDT4900_MasterThesis.Model.Db;
using Log = Serilog.Log;
using Task = System.Threading.Tasks.Task;

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
            .GraphTagged.Nodes.Select(n => simulation.Graph!.Nodes[n.NodeId].NodeId)
            .ToList();

        simulation.AlgorithmTaggedNodes = algorithmResult
            .GraphTagged.Nodes.Select(n => simulation.Graph!.Nodes[n.NodeId].NodeId)
            .ToList();

        simulation.Success = algorithmResult.Success;

        // Add EventHistory to the Simulation object
        simulation.EventHistory = simulationJob
            .Algorithm.EventHistory?.Where(e => e is NodeEvent)
            .Select(e => (NodeEvent)e)
            .Where(e => e.EventType != NodeEventType.Inhibited)
            .ToList();

        Log.Information(
            "Result from simulation: {@result}",
            new
            {
                Success = dijkstraResult.Success,
                ExecitopmTime = simulation.AlgorithmExecutionTime,
                ActualShortest = dijkstraResult.Distance,
                AlgorithmShorest = algorithmResult.Distance,
                Difference = dijkstraResult.Distance - algorithmResult.Distance,
            }
        );
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
