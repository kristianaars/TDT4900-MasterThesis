using AutoMapper;
using AutoMapper.Configuration.Annotations;
using TDT4900_MasterThesis.Algorithm.Component;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Stratium.Component;

[AutoMap(typeof(Edge))]
public class StratiumEdge : AlgorithmEdge
{
    [Ignore]
    public StratiumNode Source;

    [Ignore]
    public StratiumNode Target;

    public int GetOtherNodeId(int nodeId) => nodeId == SourceNodeId ? TargetNodeId : SourceNodeId;

    public StratiumNode GetOtherNode(StratiumNode node) => node.Equals(Source) ? Target : Source;
}
