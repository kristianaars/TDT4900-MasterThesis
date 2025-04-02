using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Handler;

public interface IAlgorithmEventConsumer
{
    public void ConsumeEvent(AlgorithmEvent algorithmEvent);
}
