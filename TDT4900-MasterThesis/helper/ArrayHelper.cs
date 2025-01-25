namespace TDT4900_MasterThesis.helpers;

public class ArrayHelper
{
    /// <summary>
    /// Fills the array with the given value.
    /// </summary>
    public static void FillArray(int[,] array, int value)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] = value;
            }
        }
    }
}
