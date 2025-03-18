using TDT4900_MasterThesis.Algorithm.Component;

namespace TDT4900_MasterThesis.Algorithm;

public class AlgorithmResult
{
    public required AlgorithmGraph<AlgorithmNode, AlgorithmEdge> GraphTagged { get; set; }
    public required int Distance { get; set; }
}
