namespace TDT4900_MasterThesis.Model.Graph;

public class NodeMessage(
    long sentAt,
    long receiveAt,
    SimulationNode? sender,
    SimulationNode receiver,
    NodeMessage.MessageType type
) : IMessage
{
    public MessageType Type { get; } = type;
    public SimulationNode? Sender { get; } = sender;
    public SimulationNode Receiver { get; } = receiver;
    public long SentAt { get; } = sentAt;
    public long ReceiveAt { get; } = receiveAt;

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
