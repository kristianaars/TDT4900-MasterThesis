using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Repository;

public class GraphRepository : BaseRepository
{
    public async Task InsertRangeAsync(
        IEnumerable<Graph> graphs,
        CancellationToken cancellationToken
    )
    {
        var dbContext = GetNewDbContext();
        await dbContext.Graphs.AddRangeAsync(graphs, cancellationToken);
        await SaveChangesAsync(dbContext);
    }
}
