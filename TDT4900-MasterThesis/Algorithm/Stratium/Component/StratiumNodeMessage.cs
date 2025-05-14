namespace TDT4900_MasterThesis.Algorithm.Stratium.Component;

public class StratiumNodeMessage
{
    public required MessageType Type { get; init; }
    public required int Charge { get; init; }
    public StratiumEdge? SourceEdge { get; init; }
    public StratiumNode? Source => SourceEdge?.GetOtherNode(Receiver);
    public required StratiumNode Receiver { get; init; }
    public required long SentAt { get; set; }
    public required long ReceiveAt { get; init; }

    public override string ToString()
    {
        return $"{nameof(Type)}: {Type}, {nameof(ReceiveAt)}: {ReceiveAt}, {nameof(SourceEdge)}: {SourceEdge}, {nameof(Receiver)}: {Receiver}";
    }

    public enum MessageType
    {
        Excitatory,
        Inhibitory,
        InitiateWave,
    }
}
