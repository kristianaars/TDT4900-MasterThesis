using System.ComponentModel.DataAnnotations;
using TDT4900_MasterThesis.Algorithm;

namespace TDT4900_MasterThesis.Model.Db;

public class Simulation : BaseModel
{
    public Graph? Graph { get; set; }

    [Required]
    public AlgorithmSpec AlgorithmSpec { get; set; }

    public Guid AlgorithmSpecId { get; set; }

    [Required]
    public GraphSpec GraphSpec { get; set; }

    public Guid GraphSpecId { get; set; }

    public Node? StartNode { get; set; }

    public Node? TargetNode { get; set; }

    public List<Node>? TaggedNodes { get; set; }

    //public List<NodeEvent>? NodeEvents { get; set; }
}
