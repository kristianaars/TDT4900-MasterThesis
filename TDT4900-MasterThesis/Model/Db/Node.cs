namespace TDT4900_MasterThesis.Model.Db;

public class Node : BaseModel
{
    public int NodeId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}

public enum NodeState
{
    Neutral,
    Refractory,
    Processing,
    Inhibited,
}
