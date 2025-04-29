using TDT4900_MasterThesis.Algorithm.Component;

namespace TDT4900_MasterThesis.Algorithm;

public class AlgorithmResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public required AlgorithmGraph<AlgorithmNode, AlgorithmEdge> GraphTagged { get; set; }
    public required int Distance { get; set; }
}
