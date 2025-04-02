using System.ComponentModel.DataAnnotations;

namespace TDT4900_MasterThesis.Model.Db;

public class NodeEvent : AlgorithmEvent
{
    [Required]
    public int NodeId { get; set; }

    [Required]
    public NodeEventType EventType { get; set; }
}

public enum NodeEventType
{
    Tagged,
    Neutral,
    Refractory,
    Processing,
    Inhibited,
}
