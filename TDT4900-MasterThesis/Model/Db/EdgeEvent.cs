namespace TDT4900_MasterThesis.Model.Db;

public class EdgeEvent : AlgorithmEvent
{
    public int SourceId { get; set; }
    public int TargetId { get; set; }
    public EdgeEventType EventType { get; set; }
    public long ReceiveAt { get; set; }
    public int Level { get; set; }
    public int Charge { get; set; } = 1;
}

public enum EdgeEventType
{
    Excitatory,
    Inhibitory,
    Neutral,
}
