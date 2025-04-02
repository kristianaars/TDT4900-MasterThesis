using TDT4900_MasterThesis.Algorithm.Component;
using TDT4900_MasterThesis.Algorithm.Utilities;
using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm;

public abstract class BaseAlgorithmAlgorithmAlgorithmAlgorithm<TNode, TEdge, TGraph>
    : BaseAlgorithmAlgorithmEventProducer,
        IAlgorithm
    where TNode : AlgorithmNode
    where TEdge : AlgorithmEdge
    where TGraph : AlgorithmGraph<TNode, TEdge>
{
    public abstract required TGraph Graph { init; get; }
    public abstract required TNode StartNode { init; get; }
    public abstract required TNode TargetNode { init; get; }

    public List<NodeEvent> EventHistory { get; } = new();
    public bool IsFinished { get; protected set; }

    public abstract void Initialize();

    public abstract void Update(long currentTick);

    public AlgorithmResult CalculateResult()
    {
        if (!IsFinished)
        {
            throw new InvalidOperationException(
                "Algorithm must be finished before calculating result"
            );
        }

        var graphTagged = Graph.GetTaggedSubgraph();
        graphTagged.Initialize();

        var verifySolution = new VerifySolution()
        {
            GraphTagged = graphTagged,
            StartNode = StartNode,
            TargetNode = TargetNode,
        };
        var distance = verifySolution.GetSolutionDistance();

        return new AlgorithmResult() { GraphTagged = graphTagged, Distance = distance };
    }

    public void ConsumeEvent(AlgorithmEvent algorithmEvent)
    {
        //EventHistory.Add(nodeEvent);
    }
}
