using TDT4900_MasterThesis.model.graph;

namespace TDT4900_MasterThesis.model;

public class NodeMessage(long receiveAt, Node? sender, Node receiver, NodeMessage.MessageType type)
    : IMessage
{
    public MessageType Type { get; } = type;
    public Node? Sender { get; } = sender;
    public Node Receiver { get; } = receiver;
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
