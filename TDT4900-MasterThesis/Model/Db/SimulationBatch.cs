using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TDT4900_MasterThesis.Model.Db;

public class SimulationBatch : BaseModel
{
    public List<Simulation>? Simulations { get; set; }

    [NotMapped]
    public bool PersistSimulations { get; set; }

    /// <summary>
    /// Specification of the algorithm to be used in the simulations.
    /// </summary>
    [Required]
    public AlgorithmSpec AlgorithmSpec { get; set; }

    /// <summary>
    /// Specification of the graph to be generated for the simulations
    /// </summary>
    [Required]
    public GraphSpec GraphSpec { get; set; }

    [Required]
    public bool RandomizeStartAndTarget { get; set; } = true;

    public int StartNodeId { get; set; } = -1;

    public int TargetNodeId { get; set; } = -1;
}
