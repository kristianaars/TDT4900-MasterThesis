namespace TDT4900_MasterThesis.Model.Db;

public class Edge : BaseModel
{
    public Node Source { get; set; }
    public Node Target { get; set; }
}
