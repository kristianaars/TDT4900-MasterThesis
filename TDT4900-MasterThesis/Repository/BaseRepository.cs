using TDT4900_MasterThesis.Context;

namespace TDT4900_MasterThesis.Repository;

public class BaseRepository
{
    protected SimulationDbContext GetNewDbContext()
    {
        var context = new SimulationDbContext();
        context.ChangeTracker.AutoDetectChangesEnabled = false;
        return context;
    }

    protected void SaveChanges(SimulationDbContext dbContext)
    {
        dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();
    }

    protected async Task SaveChangesAsync(
        SimulationDbContext dbContext,
        CancellationToken? cancellationToken = null
    )
    {
        await dbContext.SaveChangesAsync(cancellationToken ?? CancellationToken.None);
        dbContext.ChangeTracker.Clear();
        await dbContext.DisposeAsync();
    }
}
