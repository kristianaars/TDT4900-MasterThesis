using AutoMapper;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Algorithm.Alpha.Component;

[AutoMap(typeof(Edge))]
public class AlphaEdge
{
    public AlphaNode Source { get; set; }
    public AlphaNode Target { get; set; }
}
