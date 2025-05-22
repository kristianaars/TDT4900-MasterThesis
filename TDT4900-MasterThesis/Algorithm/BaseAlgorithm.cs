using GLib;
using TDT4900_MasterThesis.Algorithm.Component;
using TDT4900_MasterThesis.Algorithm.Utilities;
using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Model.Db;
using Log = Serilog.Log;

namespace TDT4900_MasterThesis.Algorithm;

public abstract class BaseAlgorithm<TNode, TEdge, TGraph>
    : BaseAlgorithmAlgorithmEventProducer,
        IAlgorithm
    where TNode : AlgorithmNode
    where TEdge : AlgorithmEdge
    where TGraph : AlgorithmGraph<TNode, TEdge>
{
    public abstract required TGraph Graph { init; get; }
    public abstract required TNode StartNode { init; get; }
    public abstract required TNode TargetNode { init; get; }

    public List<AlgorithmEvent> EventHistory { get; } = new();
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

        try
        {
            var distance = verifySolution.GetSolutionDistance();
            return new AlgorithmResult()
            {
                Success = true,
                GraphTagged = graphTagged,
                Distance = distance,
            };
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Unable to verify solution: {ErrorMessage}", e.Message);
            return new AlgorithmResult()
            {
                Success = false,
                ErrorMessage = e.Message,
                GraphTagged = graphTagged,
                Distance = -1,
            };
        }
    }

    public void ConsumeEvent(AlgorithmEvent algorithmEvent)
    {
        EventHistory.Add(algorithmEvent);
    }
}
