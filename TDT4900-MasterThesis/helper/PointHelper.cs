namespace TDT4900_MasterThesis.helpers;

public class PointHelper
{
    public static double DistanceBetween(int x1, int y1, int x2, int y2)
    {
        return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
    }
}
