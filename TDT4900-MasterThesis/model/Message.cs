using TDT4900_MasterThesis.model.graph;

namespace TDT4900_MasterThesis.model;

public class Message(long receiveAt, Node sender, Node receiver, Message.MessageType type)
{
    public MessageType Type { get; } = type;
    public long ReceiveAt { get; } = receiveAt;
    public Node Sender { get; } = sender;
    public Node Receiver { get; } = receiver;

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
