using AutoMapper;
using TDT4900_MasterThesis.Algorithm.Component;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Dijkstras.Component;

[AutoMap(typeof(Node))]
public class DijkstraNode : AlgorithmNode
{
    public int Distance { get; set; }
    public DijkstraNode? Prior { get; set; }
}
