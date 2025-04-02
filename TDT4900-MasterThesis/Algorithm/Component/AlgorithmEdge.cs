using AutoMapper;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Component;

[AutoMap(typeof(Edge))]
public class AlgorithmEdge
{
    public int SourceNodeId { get; set; }

    public int TargetNodeId { get; set; }
}
