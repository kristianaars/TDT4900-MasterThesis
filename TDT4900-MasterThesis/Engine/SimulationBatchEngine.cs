using Serilog;
using TDT4900_MasterThesis.Factory;
using TDT4900_MasterThesis.Model;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Engine;

public interface ISimulationBatchEngine
{
    Task RunSimulationBatchAsync(
        SimulationBatch simulationBatch,
        CancellationToken cancellationToken
    );
}

public class SimulationBatchEngine(
    SimulationJobFactory simulationJobFactory,
    SimulationEngine simulationEngine
) : ISimulationBatchEngine
{
    /// <summary>
    /// Runs all simulations in a simulation batch in sequence until all simulations are completed.
    /// </summary>
    /// <param name="simulationBatch">Simulation batch to run</param>
    /// <param name="cancellationToken">Cancellation token to stop the simulations</param>
    public async Task RunSimulationBatchAsync(
        SimulationBatch simulationBatch,
        CancellationToken cancellationToken
    )
    {
        var simulationJobs = new Queue<SimulationJob>();

        var numberOfSimulations = simulationBatch.Simulations.Count;

        foreach (var simulation in simulationBatch.Simulations)
        {
            simulationJobs.Enqueue(simulationJobFactory.GetSimulationJob(simulation));
        }

        while (simulationJobs.Count > 0)
        {
            var currentSimulationNumber = numberOfSimulations - simulationJobs.Count + 1;

            var simulationJob = simulationJobs.Dequeue();

            Log.Information(
                "Running simulation {CurrentSimulationNumber}/{NumberOfSimulations} with id {simulationId}",
                currentSimulationNumber,
                numberOfSimulations,
                simulationJob.Simulation.Id
            );

            await simulationEngine.RunSimulationJobAsync(simulationJob, cancellationToken);
        }
    }
}
