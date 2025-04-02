using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Handler;

public class BaseAlgorithmAlgorithmEventProducer : IAlgorithmEventProducer
{
    public AlgorithmEventHandler? EventHandler { get; set; }

    public void PostEvent(AlgorithmEvent algEvent)
    {
        EventHandler?.PostEvent(algEvent);
    }
}
