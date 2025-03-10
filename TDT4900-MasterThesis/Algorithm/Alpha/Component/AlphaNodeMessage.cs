namespace TDT4900_MasterThesis.Algorithm.Alpha.Component;

public class AlphaNodeMessage
{
    public required MessageType Type { get; set; }
    public AlphaNode? Sender { get; set; }
    public required AlphaNode Receiver { get; set; }
    public required long SentAt { get; set; }
    public required long ReceiveAt { get; set; }

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
