namespace TDT4900_MasterThesis.Model.Db;

public class Graph : BaseModel
{
    public List<Node> Nodes { get; set; }
    public List<Edge> Edges { get; set; }
    public bool IsDirected { get; set; }
}
