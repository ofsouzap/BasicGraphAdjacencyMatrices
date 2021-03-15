using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicGraphAdjacencyMatrices
{
    public static class FloydAlgorithm
    {

        /// <summary>
        /// Runs Floyd's Algorithm on an adjacency matrix to get a table for detours and a table for minimum distances
        /// </summary>
        /// <param name="matrix">The matrix to use</param>
        /// <param name="minimumDistances">The table of minimum distances</param>
        /// <returns>A matrix describing the detours that should be used for taking the shortest route to another node</returns>
        public static int[,] Run(AdjacencyMatrix matrix,
            out double[,] minimumDistances)
        {

            #region Initial distance table

            double?[,] rawMinimumDistanceTable = matrix.GetMatrixCopy();
            minimumDistances = new double[matrix.NodeCount, matrix.NodeCount];

            #region Convert no route to maximum distance

            for (int column = 0; column < matrix.NodeCount; column++)
                for (int row = 0; row < matrix.NodeCount; row++)
                    if (rawMinimumDistanceTable[column, row] == null)
                        minimumDistances[column, row] = double.MaxValue;
                    else
                        minimumDistances[column, row] = (double)rawMinimumDistanceTable[column, row];

            #endregion

            #endregion

            #region Initial route table

            int[,] routeTable = new int[matrix.NodeCount, matrix.NodeCount];

            for (int nodeToSetTo = 0; nodeToSetTo < matrix.NodeCount; nodeToSetTo++)
                for (int entry = 0; entry < matrix.NodeCount; entry++)
                    routeTable[nodeToSetTo, entry] = nodeToSetTo;

            #endregion

            #region Table construction

            for (int nodeIteration = 0; nodeIteration < matrix.NodeCount; nodeIteration++)
            {

                for (int column = 0; column < matrix.NodeCount; column++)
                {

                    if (column == nodeIteration)
                        continue;

                    for (int row = 0; row < matrix.NodeCount; row++)
                    {

                        if (row == nodeIteration)
                            continue;

                        if (column == row)
                            continue;

                        double currentDistance = minimumDistances[column, row];
                        double newPossibleSuggestedDistance = minimumDistances[column, nodeIteration] + minimumDistances[nodeIteration, row];

                        if (newPossibleSuggestedDistance < currentDistance)
                        {

                            minimumDistances[column, row] = newPossibleSuggestedDistance;
                            routeTable[column, row] = nodeIteration;

                        }

                    }

                }

            }

            #endregion

            #region Output

            return routeTable;

            #endregion

        }

    }
}
