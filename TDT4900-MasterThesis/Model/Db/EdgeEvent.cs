namespace TDT4900_MasterThesis.Model.Db;

public class EdgeEvent : AlgorithmEvent
{
    public int SourceId { get; set; }
    public int TargetId { get; set; }
    public EdgeEventType EventType { get; set; }
    public long ReceiveAt { get; set; }
}

public enum EdgeEventType
{
    Active,
    Inactive,
}
