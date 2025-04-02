using AutoMapper;
using TDT4900_MasterThesis.Algorithm;
using TDT4900_MasterThesis.Algorithm.Alpha;
using TDT4900_MasterThesis.Algorithm.Alpha.Component;
using TDT4900_MasterThesis.Algorithm.Dijkstras;
using TDT4900_MasterThesis.Algorithm.Dijkstras.Component;
using TDT4900_MasterThesis.Algorithm.Stratium;
using TDT4900_MasterThesis.Algorithm.Stratium.Component;
using TDT4900_MasterThesis.Model;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Factory;

public class SimulationJobFactory(IMapper mapper)
{
    public SimulationJob GetSimulationJob(Simulation simulation, AlgorithmSpec algorithmSpec)
    {
        IAlgorithm algorithm;

        switch (algorithmSpec.AlgorithmType)
        {
            case AlgorithmType.Alpha:
                var alphaGraph = mapper.Map<AlphaGraph>(simulation.Graph!);
                var alphaStartNode = alphaGraph.Nodes.Find(n =>
                    n.NodeId == simulation.StartNode!.NodeId
                )!;
                var alphaTargetNode = alphaGraph.Nodes.Find(n =>
                    n.NodeId == simulation.TargetNode!.NodeId
                )!;

                algorithm = new AlphaAlgorithm()
                {
                    Graph = alphaGraph,
                    StartNode = alphaStartNode,
                    TargetNode = alphaTargetNode,
                    AlgorithmSpec =
                        algorithmSpec as AlphaAlgorithmSpec
                        ?? throw new InvalidOperationException(
                            "AlgorithmSpec is not of type AlphaAlgorithmSpec."
                        ),
                };
                break;
            case AlgorithmType.Dijkstras:
                var dijkstraGraph = mapper.Map<DijkstraGraph>(simulation.Graph!);
                var dijkstraStartNode = dijkstraGraph.Nodes.Find(n =>
                    n.NodeId == simulation.StartNode!.NodeId
                )!;
                var dijkstraTargetNode = dijkstraGraph.Nodes.Find(n =>
                    n.NodeId == simulation.TargetNode!.NodeId
                )!;

                algorithm = new DijkstrasAlgorithm()
                {
                    Graph = dijkstraGraph,
                    StartNode = dijkstraStartNode,
                    TargetNode = dijkstraTargetNode,
                };
                break;
            case AlgorithmType.Stratium:
                var striatumGraph = mapper.Map<StratiumGraph>(simulation.Graph!);
                var striatumStartNode = striatumGraph.Nodes.Find(n =>
                    n.NodeId == simulation.StartNode!.NodeId
                )!;
                var striatumTargetNode = striatumGraph.Nodes.Find(n =>
                    n.NodeId == simulation.TargetNode!.NodeId
                )!;

                algorithm = new StratiumAlgorithm()
                {
                    Graph = striatumGraph,
                    StartNode = striatumStartNode,
                    TargetNode = striatumTargetNode,
                    AlgorithmSpec =
                        algorithmSpec as StratiumAlgorithmSpec
                        ?? throw new InvalidOperationException(
                            $"AlgorithmSpec with type {typeof(AlgorithmSpec)} is not of type StratiumAlgorithmSpec."
                        ),
                };
                break;
            default:
                throw new NotImplementedException(
                    $"Algorithm type {algorithmSpec.AlgorithmType} is not implemented."
                );
        }

        return new SimulationJob() { Algorithm = algorithm, Simulation = simulation };
    }
}
