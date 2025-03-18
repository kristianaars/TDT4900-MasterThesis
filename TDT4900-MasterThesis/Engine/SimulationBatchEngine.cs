using Serilog;
using TDT4900_MasterThesis.Algorithm;
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
    SimulationService simulationService,
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
        var batchSize = simulationBatch.Simulations.Count;
        var graphGenerationSpec = simulationBatch.GraphSpec;
        var algorithmSpec = simulationBatch.AlgorithmSpec;

        // Build queue of simulations to run
        var simulationQueue = new Queue<Simulation>();
        foreach (var simulation in simulationBatch.Simulations)
        {
            simulationQueue.Enqueue(simulation);
        }

        // Save the simulation batch "skeleton" data to database
        if (simulationBatch.PersistSimulations)
        {
            await simulationPersistenceService.SaveSimulationBatchAsync(
                simulationBatch,
                cancellationToken
            );
        }

        // Clear reference to simulations to invoke garbage collection
        simulationBatch.Simulations.Clear();

        Log.Information(
            "Starting simulation batch with id {simulationBatchId} containing {batchSize} simulations",
            simulationBatch.Id,
            batchSize
        );
        Log.Information(
            "Algorithm specifications for {simulationBatchId}: {@algorithmSpec}",
            simulationBatch.Id,
            algorithmSpec
        );
        Log.Information(
            "Graph generation specifications for simulation batch {simulationBatchId}: {@graphSpec}",
            simulationBatch.Id,
            graphGenerationSpec
        );
        Log.Information(
            "Persistence is {persist} for this simulation batch {simulationBatchId}",
            simulationBatch.PersistSimulations ? "enabled" : "disabled",
            simulationBatch.Id
        );

        // Update the simulation stats view model
        simulationStatsViewModel.SimulationBatchId = simulationBatch.Id;
        simulationStatsViewModel.SimulationBatchSize = batchSize;

        var persistenceTasks = new List<Task>();
        var unsavedSimulations = new List<Simulation>();
        var unsavedSimulationsLock = new Lock();

        while (simulationQueue.Count > 0 && !cancellationToken.IsCancellationRequested)
        {
            var currentSimulationNumber = batchSize - simulationQueue.Count + 1;
            var simulation = simulationQueue.Dequeue();

            // Generate the graph and set random source and target node
            simulation.Graph = graphFactory.CreateGraph(graphGenerationSpec);
            simulation.StartNode = simulation.TargetNode = simulation.Graph.Nodes[
                new Random().Next(simulation.Graph.Nodes.Count)
            ];
            while (simulation.TargetNode == simulation.StartNode)
            {
                simulation.TargetNode = simulation.Graph.Nodes[
                    new Random().Next(simulation.Graph.Nodes.Count)
                ];
            }

            Log.Information(
                "Running simulation {CurrentSimulationNumber}/{NumberOfSimulations} with id {simulationId}",
                currentSimulationNumber,
                batchSize,
                simulation.Id
            );
            // Build the simulation job
            await simulationService.RunSimulationWithVerificationAsync(
                simulation,
                algorithmSpec,
                cancellationToken
            );

            // Wait for persistence tasks to complete before continuing if above 10 concurrent tasks
            var activePersistenceTaskCount = persistenceTasks.Count(t => !t.IsCompleted);
            if (activePersistenceTaskCount > 5)
            {
                Log.Information(
                    "Waiting for {activePersistenceTaskCount} persistence tasks to complete before continuing...",
                    activePersistenceTaskCount
                );

                simulationStatsViewModel.SimulationState = "Persisting simulation data...";

                await Task.WhenAll(persistenceTasks);
            }

            lock (unsavedSimulationsLock)
            {
                if (simulationBatch.PersistSimulations)
                    unsavedSimulations.Add(simulation);

                // Persist the simulations to database every 15 simulations
                if (
                    simulationBatch.PersistSimulations
                        && (currentSimulationNumber % 15 == 0 || simulationQueue.Count == 0)
                    || cancellationToken.IsCancellationRequested
                )
                {
                    persistenceTasks.Add(
                        Task.Run(async () =>
                        {
                            //simulationStatsViewModel.SimulationState = "Persisting data...";

                            List<Simulation> sim;
                            lock (unsavedSimulationsLock)
                            {
                                sim = unsavedSimulations;
                                unsavedSimulations = [];
                            }

                            var simCount = sim.Count;

                            Log.Information(
                                "Persisting {unsavedSimulations} simulations to db...",
                                simCount
                            );

                            await simulationPersistenceService.InsertSimulationRangeAsync(
                                sim,
                                simulationBatch,
                                cancellationToken
                            );

                            Log.Information(
                                "{simCount} simulations were persisted to db",
                                simCount
                            );
                        })
                    );
                }
            }

            simulationStatsViewModel.CompletedSimulations = currentSimulationNumber;
        }

        Log.Information(
            "Simulation batch with id {simulationBatchId} {status} after {completedSimulations} simulations",
            simulationBatch.Id,
            cancellationToken.IsCancellationRequested ? "was cancelled" : "completed",
            batchSize - simulationQueue.Count
        );

        var totalTasks = persistenceTasks.Count;
        var remainingTasks = totalTasks;
        while (persistenceTasks.Count > 0)
        {
            Task completedTask = await Task.WhenAny(persistenceTasks);
            persistenceTasks.Remove(completedTask);
            remainingTasks--;

            var remainingPercent = $"{(1.0 - remainingTasks / (double)totalTasks) * 100.0f:F2}%";

            Log.Information(
                "{remainingTasks} persistence tasks remaining... ({remainingPercent})",
                remainingTasks,
                remainingPercent
            );

            simulationStatsViewModel.SimulationState =
                $"Persisting simulations... ({remainingPercent})";
        }

        if (cancellationToken.IsCancellationRequested)
        {
            simulationStatsViewModel.SimulationState = "Simulation batch was cancelled";
        }
        else if (totalTasks > 0)
        {
            simulationStatsViewModel.SimulationState = "Simulation batch persisted";
        }
        else
        {
            simulationStatsViewModel.SimulationState = "Simulation batch completed";
        }
    }
}
