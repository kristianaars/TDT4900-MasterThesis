namespace TDT4900_MasterThesis.Helper;

public class ArrayHelper
{
    /// <summary>
    /// Fills the array with the given value.
    /// </summary>
    public static void FillArray<T>(T[,] array, T value)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] = value;
            }
        }
    }

    /// <summary>
    /// Fills the array with the given value.
    /// </summary>
    public static void FillArray<T>(T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
    }
}
