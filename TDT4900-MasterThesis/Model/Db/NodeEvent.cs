using System.ComponentModel.DataAnnotations;

namespace TDT4900_MasterThesis.Model.Db;

public class NodeEvent : BaseModel
{
    [Required]
    public int NodeId { get; set; }

    [Required]
    public long? Tick { get; set; } = null;

    [Required]
    public EventType EventType { get; set; }
}

public enum EventType
{
    Tagged,
    Neutral,
    Refractory,
    Processing,
    Inhibited,
}
