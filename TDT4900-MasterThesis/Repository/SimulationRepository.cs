using EFCore.BulkExtensions;
using TDT4900_MasterThesis.Context;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Repository;

public class SimulationRepository : IBaseRepository<Simulation>
{
    private SimulationDbContext GetNewDbContext() => new();

    public IEnumerable<Simulation> List() => GetNewDbContext().Simulations;

    public async Task<Simulation> GetByIdAsync(Guid id) =>
        await GetNewDbContext().Simulations.FindAsync(id)
        ?? throw new KeyNotFoundException($"Simulation Batch with id {id} not found");

    public async Task InsertAsync(Simulation simulation)
    {
        var dbContext = GetNewDbContext();
        await dbContext.Simulations.AddAsync(simulation);
        await SaveChangesAsync(dbContext);
    }

    public void Delete(Simulation simulation)
    {
        using var dbContext = GetNewDbContext();
        dbContext.Simulations.Remove(simulation);
        SaveChanges(dbContext);
    }

    public void Update(Simulation simulation)
    {
        using var dbContext = GetNewDbContext();
        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
        dbContext.Simulations.Update(simulation);
        SaveChanges(dbContext);
    }

    public async Task UpdateRangeAsync(ICollection<Simulation> simulations)
    {
        await using var dbContext = GetNewDbContext();
        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
        dbContext.Simulations.UpdateRange(simulations.ToList());
        await SaveChanges(dbContext);
    }

    private async Task SaveChangesAsync(SimulationDbContext dbContext)
    {
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }

    private async Task SaveChanges(SimulationDbContext dbContext)
    {
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }
}
