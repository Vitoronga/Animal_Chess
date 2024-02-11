using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalChessLibrary
{
    public static class CollectionHelper
    {
        internal static List<T> GetListFromBidimensionalArray<T> (T[,] array)
        {
            int lengthDim1 = array.GetLength(0);
            int lengthDim2 = array.GetLength(1);

            T[] singleDimensionArray = new T[lengthDim1 * lengthDim2];

            for (int i = 0; i < lengthDim1; i++)
            {
                for (int j = 0; j < lengthDim2; j++)
                {
                    singleDimensionArray[i * lengthDim2 + j] = array[i, j];
                }
            }

            return singleDimensionArray.ToList();
        }

        public static List<T> GetListFromGameBoard<T>(T[,] array)
        {
            int lengthDim1 = array.GetLength(0);
            int lengthDim2 = array.GetLength(1);

            T[] singleDimensionArray = new T[lengthDim1 * lengthDim2];

            for (int y = 0; y < lengthDim2; y++)
            {
                for (int x = 0; x < lengthDim1; x++)
                {
                    singleDimensionArray[y * lengthDim1 + x] = array[x, lengthDim2 - 1 - y];
                }
            }

            return singleDimensionArray.ToList();
        }

        internal static T[,] GetBidimensionalArrayFromList<T> (List<T> list, int lengthDim1, int lengthDim2)
        {
            if (lengthDim1 * lengthDim2 != list.Count) throw new ArgumentOutOfRangeException();

            T[,] array = new T[lengthDim1, lengthDim2];

            for (int i = 0; i < lengthDim1; i++)
            {
                for (int j = 0; j < lengthDim2; j++)
                {
                    array[i, j] = list[i*lengthDim2 + j];
                }
            }

            return array;
        }
    }
}
