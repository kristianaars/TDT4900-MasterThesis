using Serilog;
using TDT4900_MasterThesis.Factory;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Service;
using TDT4900_MasterThesis.ViewModel;

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
    SimulationEngine simulationEngine,
    SimulationStatsViewModel simulationStatsViewModel,
    SimulationPersistenceService simulationPersistenceService,
    GraphFactory graphFactory
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
        simulationBatch.Id = Guid.NewGuid();

        var simulationQueue = new Queue<Simulation>();

        var batchSize = simulationBatch.Simulations.Count;

        foreach (var simulation in simulationBatch.Simulations)
        {
            simulationQueue.Enqueue(simulation);
        }

        simulationStatsViewModel.SimulationBatchId = simulationBatch.Id;
        simulationStatsViewModel.SimulationBatchSize = batchSize;

        Log.Information(
            "Starting simulation batch with id {simulationBatchId} and {batchSize} simulations",
            simulationBatch.Id,
            batchSize
        );
        Log.Information(
            "Persistence is {persist} for this simulation batch",
            simulationBatch.PersistSimulations
        );

        if (simulationBatch.PersistSimulations)
        {
            await simulationPersistenceService.SaveSimulationBatchAsync(simulationBatch);
        }

        var unsavedSimulations = new List<Simulation>();

        while (simulationQueue.Count > 0)
        {
            var currentSimulationNumber = batchSize - simulationQueue.Count + 1;

            var simulation = simulationQueue.Peek();

            // Generate the graph and set random source and target node
            simulation.Graph = graphFactory.CreateGraph(simulation.GraphSpec);
            simulation.StartNode = simulation.Graph.Nodes[
                new Random().Next(simulation.Graph.Nodes.Count)
            ];
            simulation.TargetNode = simulation.Graph.Nodes[
                new Random().Next(simulation.Graph.Nodes.Count)
            ];

            // Build the simulation job
            var simulationJob = simulationJobFactory.GetSimulationJob(simulationQueue.Dequeue());

            Log.Information(
                "Running simulation {CurrentSimulationNumber}/{NumberOfSimulations} with id {simulationId}",
                currentSimulationNumber,
                batchSize,
                simulationJob.Simulation.Id
            );

            await simulationEngine.RunSimulationJobAsync(simulationJob, cancellationToken);

            unsavedSimulations.Add(simulation);

            // Persist the simulations to database every 25 simulations
            if (
                simulationBatch.PersistSimulations
                && (currentSimulationNumber % 25 == 0 || simulationQueue.Count == 0)
            )
            {
                Log.Information(
                    "Persisting {unsavedSimulations} simulations to db...",
                    unsavedSimulations.Count
                );

                simulationStatsViewModel.SimulationState = "Persisting data...";

                await simulationPersistenceService.UpdateSimulationRangeAsync(unsavedSimulations);
                unsavedSimulations.Clear();

                Log.Information("Simulations were persisted to db");
            }

            simulationStatsViewModel.CompletedSimulations = currentSimulationNumber;
        }

        Log.Information(
            "Simulation batch with id {simulationBatchId} completed",
            simulationBatch.Id
        );

        simulationStatsViewModel.SimulationState = "Batch completed";
    }
}
