using System.Collections;
using TDT4900_MasterThesis.model;

namespace TDT4900_MasterThesis;

public class HighwayHierarchyAlgorithm
{
    /// <summary>
    /// Holds the current search level of each node n in the graph.
    /// </summary>
    private int[] _nodeLevel;
    
    /// <summary>
    /// Holds the distance to the edge of the current neighbourhood for node n.
    /// </summary>
    private int[] _gap;

    /// <summary>
    /// Holds the radius of node n at level l. Lookup is done by <see cref="_radius"/>[l][n]
    /// </summary>
    private int[][] _radius;
    
    
    private Hashtable _edgeLevel;
    
}