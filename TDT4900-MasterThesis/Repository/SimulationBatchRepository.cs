using EFCore.BulkExtensions;
using TDT4900_MasterThesis.Context;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Repository;

public class SimulationBatchRepository() : IBaseRepository<SimulationBatch>
{
    private SimulationDbContext GetNewDbContext() => new();

    public IEnumerable<SimulationBatch> List() => GetNewDbContext().SimulationBatches;

    public async Task<SimulationBatch> GetByIdAsync(Guid id) =>
        await GetNewDbContext().SimulationBatches.FindAsync(id)
        ?? throw new KeyNotFoundException($"Simulation Batch with id {id} not found");

    public async Task InsertAsync(SimulationBatch simulationBatch)
    {
        await using var dbContext = GetNewDbContext();
        await dbContext.SimulationBatches.AddAsync(simulationBatch);
        SaveChanges(dbContext);
    }

    public void Delete(SimulationBatch simulationBatch)
    {
        using var dbContext = GetNewDbContext();
        dbContext.SimulationBatches.Remove(simulationBatch);
        SaveChanges(dbContext);
    }

    public void Update(SimulationBatch simulationBatch)
    {
        using var dbContext = GetNewDbContext();
        dbContext.SimulationBatches.Update(simulationBatch);
        SaveChanges(dbContext);
    }

    private void SaveChanges(SimulationDbContext dbContext)
    {
        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
        dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();
    }
}
