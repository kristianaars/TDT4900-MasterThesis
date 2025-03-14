using AutoMapper;
using AutoMapper.Configuration.Annotations;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Dijkstras.Component;

[AutoMap(typeof(Edge))]
public class DijkstraEdge
{
    [SourceMember("Source.NodeId")]
    public int SourceId { get; set; }

    [SourceMember("Target.NodeId")]
    public int TargetId { get; set; }
}
