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
        var simulationQueue = new Queue<Simulation>();

        var batchSize = simulationBatch.Simulations.Count;

        foreach (var simulation in simulationBatch.Simulations)
        {
            simulationQueue.Enqueue(simulation);
        }

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
            await simulationPersistenceService.SaveSimulationBatchAsync(
                simulationBatch,
                cancellationToken
            );
        }

        // Clear reference to simulations to improve garbage collection
        simulationBatch.Simulations.Clear();

        simulationStatsViewModel.SimulationBatchId = simulationBatch.Id;
        simulationStatsViewModel.SimulationBatchSize = batchSize;

        var persistanceTasks = new List<Task>();
        var unsavedSimulations = new List<Simulation>();

        var unsSumLock = new Lock();

        while (simulationQueue.Count > 0 && !cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();

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

            // Wait for persistence tasks to complete before continuing if above 10 concurrent tasks
            var activePersistenceTaskCount = persistanceTasks.Count(t => !t.IsCompleted);
            if (activePersistenceTaskCount > 5)
            {
                Log.Information(
                    "Waiting for {activePersistenceTaskCount} persistence tasks to complete before continuing...",
                    activePersistenceTaskCount
                );

                simulationStatsViewModel.SimulationState = "Persisting simulation data...";

                await Task.WhenAll(persistanceTasks);
            }

            lock (unsSumLock)
            {
                if (simulationBatch.PersistSimulations)
                    unsavedSimulations.Add(simulation);

                // Persist the simulations to database every 25 simulations
                if (
                    simulationBatch.PersistSimulations
                    && (currentSimulationNumber % 15 == 0 || simulationQueue.Count == 0)
                )
                {
                    persistanceTasks.Add(
                        Task.Run(
                            async () =>
                            {
                                //simulationStatsViewModel.SimulationState = "Persisting data...";

                                List<Simulation> sim;
                                lock (unsSumLock)
                                {
                                    sim = unsavedSimulations;
                                    unsavedSimulations = [];
                                }

                                var simCount = sim.Count;

                                Log.Information(
                                    "Persisting {unsavedSimulations} simulations to db...",
                                    simCount
                                );

                                await simulationPersistenceService.UpdateSimulationRangeDataAsync(
                                    sim,
                                    cancellationToken
                                );

                                Log.Information(
                                    "{simCount} simulations were persisted to db",
                                    simCount
                                );
                            },
                            cancellationToken
                        )
                    );
                }
            }

            simulationStatsViewModel.CompletedSimulations = currentSimulationNumber;
        }

        Log.Information(
            "Simulation batch with id {simulationBatchId} completed",
            simulationBatch.Id
        );

        var totalTasks = persistanceTasks.Count;
        var remainingTasks = totalTasks;
        while (persistanceTasks.Count > 0 && !cancellationToken.IsCancellationRequested)
        {
            Task completedTask = await Task.WhenAny(persistanceTasks);
            persistanceTasks.Remove(completedTask);
            remainingTasks--;

            var remainingPercent = $"{(1.0 - remainingTasks / (double)totalTasks) * 100.0f:F2}%";

            Log.Information(
                "{remainingTasks} persistence tasks remaining... ({remainingPercent})",
                remainingTasks,
                remainingTasks
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
