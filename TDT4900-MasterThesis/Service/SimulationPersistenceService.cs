using EFCore.BulkExtensions;
using TDT4900_MasterThesis.Context;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Repository;

namespace TDT4900_MasterThesis.Service;

public class SimulationPersistenceService(
    SimulationBatchRepository simulationBatchRepository,
    SimulationRepository simulationRepository,
    GraphRepository graphRepository
)
{
    public async Task<SimulationBatch> SaveSimulationBatchAsync(
        SimulationBatch simulationBatch,
        CancellationToken? cancellationToken = null
    )
    {
        await simulationBatchRepository.InsertAsync(simulationBatch, cancellationToken);
        return simulationBatch;
    }

    public void UpdateSimulation(Simulation simulation)
    {
        simulationRepository.Update(simulation);
    }

    public async Task UpdateSimulationRangeAsync(
        ICollection<Simulation> simulations,
        CancellationToken? cancellationToken = null
    )
    {
        await simulationRepository.UpdateRangeAsync(simulations, cancellationToken);
    }

    /// <summary>
    /// Updates underlyying simulation data such as Graph using Insert instead of Update
    /// This allows for more efficient data-insertion
    /// </summary>
    /// <param name="simulations"></param>
    /// <param name="cancellationToken"></param>
    public async Task UpdateSimulationRangeDataAsync(
        ICollection<Simulation> simulations,
        CancellationToken cancellationToken
    )
    {
        // Insert graph data
        await graphRepository.InsertRangeAsync(
            simulations.Select(s => s.Graph!),
            cancellationToken
        );
    }
}
