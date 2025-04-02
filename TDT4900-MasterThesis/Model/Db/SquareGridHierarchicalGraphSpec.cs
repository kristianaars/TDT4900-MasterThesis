namespace TDT4900_MasterThesis.Model.Db;

public class SquareGridHierarchicalGraphSpec : GraphSpec
{
    public int Distance { get; set; }
    public int Noise { get; set; }

    public int BaseGridSize { get; set; }
    public int HierarchicalLevels { get; set; }
}
