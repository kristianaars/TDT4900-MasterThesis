using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TDT4900_MasterThesis.Model;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Context;

public class SimulationDbContext : DbContext
{
    public DbSet<Node> Nodes { get; set; }
    public DbSet<Graph> Graphs { get; set; }
    public DbSet<Simulation> Simulations { get; set; }
    public DbSet<SimulationBatch> SimulationBatches { get; set; }

    public DbSet<GraphSpec> GraphSpecs { get; set; }
    public DbSet<NeighboringGraphSpec> NeighboringGraphSpecs { get; set; }

    public DbSet<AlgorithmSpec> AlgorithmSpecs { get; set; }
    public DbSet<AlphaAlgorithmSpec> AlphaAlgorithmSpecs { get; set; }

    public DbSet<NodeEvent> NodeEvents { get; set; }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql($"Host=localhost;Database=tdt4900;Username=postgres;Password=postgres");
    }
}
