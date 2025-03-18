namespace TDT4900_MasterThesis.Model.Db;

public class Edge : BaseModel
{
    public int SourceNodeId { get; set; }
    public int TargetNodeId { get; set; }
    public bool IsDirected { get; set; }
}
