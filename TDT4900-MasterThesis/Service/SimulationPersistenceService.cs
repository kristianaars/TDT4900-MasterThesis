using EFCore.BulkExtensions;
using TDT4900_MasterThesis.Context;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Repository;

namespace TDT4900_MasterThesis.Service;

public class SimulationPersistenceService
{
    private readonly SimulationBatchRepository _simulationBatchRepository;
    private readonly SimulationRepository _simulationRepository;

    public SimulationPersistenceService(
        SimulationBatchRepository simulationBatchRepository,
        SimulationRepository simulationRepository
    )
    {
        _simulationBatchRepository = simulationBatchRepository;
        _simulationRepository = simulationRepository;
    }

    public async Task<SimulationBatch> SaveSimulationBatchAsync(SimulationBatch simulationBatch)
    {
        await _simulationBatchRepository.InsertAsync(simulationBatch);
        return simulationBatch;
    }

    public void UpdateSimulation(Simulation simulation)
    {
        _simulationRepository.Update(simulation);
    }

    public async Task UpdateSimulationRangeAsync(ICollection<Simulation> simulations)
    {
        await _simulationRepository.UpdateRangeAsync(simulations);
    }
}
