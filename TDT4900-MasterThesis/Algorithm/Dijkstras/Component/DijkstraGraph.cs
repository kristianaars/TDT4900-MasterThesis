using AutoMapper;
using TDT4900_MasterThesis.Algorithm.Component;
using TDT4900_MasterThesis.Helper;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Dijkstras.Component;

[AutoMap(typeof(Graph))]
public class DijkstraGraph : AlgorithmGraph<DijkstraNode, DijkstraEdge> { }
