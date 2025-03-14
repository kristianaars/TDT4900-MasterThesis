using AutoMapper;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Dijkstras.Component;

[AutoMap(typeof(Node))]
public class DijkstraNode
{
    public int NodeId { get; set; }
    public int Distance { get; set; }
    public int Weight { get; set; }
    public DijkstraNode? Prior { get; set; }
    public bool IsTagged { get; set; }
}
