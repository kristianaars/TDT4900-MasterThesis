using System.ComponentModel.DataAnnotations.Schema;

namespace TDT4900_MasterThesis.Model.Db;

public class Graph : BaseModel
{
    public List<Node> Nodes { get; set; }

    [NotMapped]
    public List<Edge> Edges { get; set; }

    public bool IsDirected { get; set; }
    public int Levels { get; set; } = 1;
}
