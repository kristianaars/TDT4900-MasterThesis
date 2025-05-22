using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Repository;

namespace TDT4900_MasterThesis.Service;

public class SimulationPersistenceService(
    SimulationBatchRepository simulationBatchRepository,
    SimulationRepository simulationRepository,
    EventHistoryRepository eventHistoryRepository
)
{
    public async Task SaveSimulationBatchAsync(
        SimulationBatch simulationBatch,
        CancellationToken? cancellationToken = null
    )
    {
        await simulationBatchRepository.InsertAsync(simulationBatch, cancellationToken);
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

    public async Task InsertSimulationRangeAsync(
        ICollection<Simulation> simulations,
        SimulationBatch simulationBatch,
        CancellationToken? cancellationToken = null
    )
    {
        await simulationRepository.InsertRangeAsync(simulations, cancellationToken);

        foreach (var simulation in simulations)
        {
            await eventHistoryRepository.PersistNodeEventListAsync(
                simulation.Id,
                simulation.EventHistory,
                cancellationToken
            );
        }
    }
}
