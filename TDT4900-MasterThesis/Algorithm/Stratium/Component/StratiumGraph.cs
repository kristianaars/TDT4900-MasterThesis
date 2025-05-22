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

        var mInit = 50;
        var tasks = new List<Task>();

        for (int i = 0; i * mInit < Nodes.Count; i++)
        {
            var rest = Nodes.Count - i * mInit;
            var initNodes = Nodes.Slice(i * mInit, Math.Min(mInit, rest));
            tasks.Add(
                Task.Run(() =>
                {
                    initNodes.ForEach(n =>
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
                        n.MaxSearchLevel = GetOutEdges(n).Max(e => e.Level);
                    });
                })
            );
        }

        Task.WaitAll(tasks.ToArray());
    }
}
