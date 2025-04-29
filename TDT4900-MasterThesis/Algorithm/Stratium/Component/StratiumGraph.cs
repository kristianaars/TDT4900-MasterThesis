using AutoMapper;
using TDT4900_MasterThesis.Algorithm.Component;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Stratium.Component;

[AutoMap(typeof(Graph))]
public class StratiumGraph : AlgorithmGraph<StratiumNode, StratiumEdge>
{
    /// <summary>
    /// Configure fields of Nodes and Edges as well as the backing adjecency matrix.
    /// </summary>
    public void Initialize(StratiumAlgorithmSpec algorithmSpec)
    {
        base.Initialize();

        // Initialize all nodes
        Nodes.ForEach(n =>
        {
            n.Neighbours = GetOutNeighbours(n).ToList();
            n.NeighbouringEdges = GetOutEdges(n).ToList();
            n.AllNodes = Nodes;
            n.RefractoryPeriod = algorithmSpec.RefractoryPeriod;
            n.DeltaExcitatory = algorithmSpec.DeltaTExcitatory;
            n.DeltaInhibitory = algorithmSpec.DeltaTInhibitory;
            n.TauZero = algorithmSpec.TauZero;
            n.TauPlus = algorithmSpec.TauPlus;
            n.Tau = n.TauZero;
            n.SearchLevel = n.NeighbouringEdges.Max(e => e.Level as int?) ?? 0; // Begin search at the highest possible level for forward sweeeps
        });
    }
}
