using System;

namespace InterpolationLib
{
    static class Utils
    {
        private static T[,] Transpose<T>(this T[,] self)
        {
            int width = self.GetLength(0);
            int height = self.GetLength(1);
            T[,] result = new T[height, width];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    result[j, i] = self[i, j];
            return result;
        }

        private static T[,] GoThrough2DHalf<T>(this T[,] self, Func<T[], T[]> rowSelector)
        {
            int width = self.GetLength(0);
            int height = self.GetLength(1);
            T[][] resultColumns = new T[width][];
            for (int i = 0; i < width; i++)
            {
                T[] nodeColumn = new T[height];
                for (int j = 0; j < height; j++)
                    nodeColumn[j] = self[i, j];
                T[] resultColumn = rowSelector(nodeColumn);
                resultColumns[i] = resultColumn;
            }

            T[] firstResultColumn = resultColumns[0];
            int resultHeight = firstResultColumn.Length;
            T[,] result = new T[width, resultHeight];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < resultHeight; j++)
                    result[i, j] = resultColumns[i][j];
            return result.Transpose();
        }

        public static T[,] GoThrough2D<T>(this T[,] self, Func<T[], T[]> rowSelector)
        {
            return self.GoThrough2DHalf(rowSelector).GoThrough2DHalf(rowSelector);
        }

        public static T[,,] GoThrough3D<T>(this T[,,] self, Func<T[], T[]> rowSelector)
        {
            int width = self.GetLength(0);
            int height = self.GetLength(1);
            int depth = self.GetLength(2);

            T[][,] firstResult = new T[width][,];
            for (int i = 0; i < width; i++)
            {
                T[,] slice = new T[height, depth];
                for (int j = 0; j < height; j++)
                    for (int k = 0; k < depth; k++)
                        slice[j, k] = self[i, j, k];
                T[,] resultSlice = slice.GoThrough2DHalf(rowSelector);
                firstResult[i] = resultSlice;
            }

            T[,] firstSlice = firstResult[0];
            int resultHeight = firstSlice.GetLength(0);
            int resultDepth = firstSlice.GetLength(1);

            T[,][] midResult = new T[resultHeight, resultDepth][];
            for (int i = 0; i < resultHeight; i++)
                for (int j = 0; j < resultDepth; j++)
                {
                    T[] nodes = new T[width];
                    for (int k = 0; k < width; k++)
                        nodes[k] = firstResult[k][i, j];
                    T[] resultNodes = rowSelector(nodes);
                    midResult[i, j] = resultNodes;
                }

            T[] firstEndSlice = midResult[0, 0];
            int resultWidth = firstEndSlice.Length;
            T[,,] endResult = new T[resultWidth, resultHeight, resultDepth];
            for (int i = 0; i < resultWidth; i++)
                for (int j = 0; j < resultHeight; j++)
                    for (int k = 0; k < resultDepth; k++)
                        endResult[i, j, k] = midResult[j, k][i];
            return endResult;
        }
    }
}
