namespace TDT4900_MasterThesis.Algorithm.Alpha.Component;

public class AlphaNodeMessage
{
    public required MessageType Type { get; init; }
    public AlphaNode? Sender { get; init; }
    public required AlphaNode Receiver { get; init; }
    public required long SentAt { get; set; }
    public required long ReceiveAt { get; init; }

    public override string ToString()
    {
        return $"{nameof(Type)}: {Type}, {nameof(ReceiveAt)}: {ReceiveAt}, {nameof(Sender)}: {Sender}, {nameof(Receiver)}: {Receiver}";
    }

    public enum MessageType
    {
        Excitatory,
        Inhibitory,
    }
}
