using System.ComponentModel.DataAnnotations;
using TDT4900_MasterThesis.Algorithm;

namespace TDT4900_MasterThesis.Model.Db;

public class Simulation : BaseModel
{
    [Required]
    public Graph? Graph { get; set; }

    [Required]
    public AlgorithmSpec AlgorithmSpec { get; set; }

    [Required]
    public Node? StartNode { get; set; }

    [Required]
    public Node? TargetNode { get; set; }

    [Required]
    public List<Node>? TaggedNodes { get; set; }

    [Required]
    public List<NodeEvent>? NodeEvents { get; set; }
}
