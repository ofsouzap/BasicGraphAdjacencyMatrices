using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicGraphAdjacencyMatrices
{
    public static class TravelingSalesman
    {

        /// <summary>
        /// Finds an initial upper bound using a minimum spanning tree for the Traveling Salesman Problem on a provided matrix. This doesn't try and use any shortcuts to minimise the bound
        /// </summary>
        /// <param name="matrix">The matrix to consider</param>
        /// <returns>The initial upper bound for the length of the solution</returns>
        public static double FindMSTUpperBound(AdjacencyMatrix _matrix)
        {

            if (!_matrix.IsUndirected)
            {
                throw new ArgumentException("Provided matrix was directed");
            }

            AdjacencyMatrix matrix = _matrix.GenerateTableOfLeastDistances();

            int[][] mstEdges = PrimAlgorithm.Run(matrix);

            double totalMSTLength = 0;

            foreach (int[] edge in mstEdges)
                totalMSTLength += (double)matrix[edge[0], edge[1]];

            return totalMSTLength * 2;

        }

        /// <summary>
        /// Finds a lower bound using a minimum spanning tree for the Traveling Salesman Problem on a provided matrix
        /// </summary>
        /// <param name="matrix">The matrix to consider</param>
        /// <returns>The lower bound for the length of the solution</returns>
        public static double FindMSTLowerBound(AdjacencyMatrix _matrix)
        {

            if (!_matrix.IsUndirected)
            {
                throw new ArgumentException("Provided matrix was directed");
            }

            AdjacencyMatrix matrix = _matrix.GenerateTableOfLeastDistances();

            double maximumDistance = 0;

            for (int consideredNode = 0; consideredNode < matrix.NodeCount; consideredNode++)
            {

                #region Making new Matrix

                double?[,] newMatrixValues = new double?[
                    matrix.NodeCount - 1,
                    matrix.NodeCount - 1
                    ];

                int columnIndex = 0;
                int rowIndex = 0;
                for (int column = 0; column < matrix.NodeCount; column++)
                {

                    if (column == consideredNode)
                        continue;

                    for (int row = 0; row < matrix.NodeCount; row++)
                    {

                        if (row == consideredNode)
                            continue;

                        newMatrixValues[columnIndex, rowIndex] = matrix[column, row];

                        rowIndex++;

                    }

                    rowIndex = 0;
                    columnIndex++;

                }

                AdjacencyMatrix reducedMatrix = new AdjacencyMatrix(newMatrixValues);

                #endregion

                #region Residual Minimum Spanning Tree

                int[][] rmstEdges = PrimAlgorithm.Run(reducedMatrix);

                double totalRMSTLength = 0;

                foreach (int[] edge in rmstEdges)
                    totalRMSTLength += (double)reducedMatrix[edge[0], edge[1]];

                #endregion

                #region Extra Arcs Addition

                double minimumLength = double.MaxValue;
                double secondMinimumLength = double.MaxValue;

                for (int otherNode = 0; otherNode < matrix.NodeCount; otherNode++)
                {

                    if (otherNode == consideredNode)
                        continue;

                    double? entry = matrix[consideredNode, otherNode];

                    if (entry == null)
                        continue;

                    double value = (double)entry;

                    if (value < minimumLength)
                    {
                        secondMinimumLength = minimumLength;
                        minimumLength = value;
                    }
                    else if (value < secondMinimumLength)
                    {
                        secondMinimumLength = value;
                    }

                }

                #endregion

                #region Conclusion

                double finalDistance = totalRMSTLength + minimumLength + secondMinimumLength;

                if (finalDistance > maximumDistance)
                    maximumDistance = finalDistance;

                #endregion

            }

            return maximumDistance;

        }

        /// <summary>
        /// Finds an upper bound for the Traveling Salesman Problem using the nearest neighbour algorithm
        /// </summary>
        /// <param name="_matrix">The matrix to consider</param>
        /// <returns>The upper bound found for the length of the solution</returns>
        public static double FindNearestNeighbourUpperBound(AdjacencyMatrix _matrix)
        {

            if (!_matrix.IsUndirected)
            {
                throw new ArgumentException("Provided matrix was directed");
            }

            AdjacencyMatrix matrix = _matrix.GenerateTableOfLeastDistances();

            double minimumUpperBound = double.MaxValue;

            for (int considerationNode = 0; considerationNode < matrix.NodeCount; considerationNode++)
            {

                int currentNode = considerationNode;

                bool[] nodesVisited = new bool[matrix.NodeCount];
                nodesVisited[considerationNode] = true;

                double totalConsiderationDistance = 0;

                #region Visit All Nodes

                while (!nodesVisited.All(x => x))
                {

                    #region Decide Next Node

                    int minimumNextNode = -1;
                    double minimumEdgeDistance = double.MaxValue;

                    for (int otherNode = 0; otherNode < matrix.NodeCount; otherNode++)
                    {

                        if (otherNode == currentNode) //If the node is the same
                            continue;

                        if (nodesVisited[otherNode]) //If the node has already been visited
                            continue;

                        double? entry = matrix[currentNode, otherNode];

                        if (entry == null) //If there is no route (this shouldn't occur as a table of minimum distances is being used)
                            continue;

                        double distance = (double)entry;

                        if (distance < minimumEdgeDistance)
                        {
                            //Replace current minimum distance with the current otherNode and its edge
                            minimumEdgeDistance = distance;
                            minimumNextNode = otherNode;
                        }

                    }

                    #endregion

                    #region "Move" to Next Node

                    currentNode = minimumNextNode;
                    nodesVisited[currentNode] = true;

                    totalConsiderationDistance += minimumEdgeDistance;

                    #endregion

                }

                #endregion

                #region Adding Return Path

                DijkstraAlgorithm.Run(matrix, currentNode, considerationNode, out double returnDistance);

                totalConsiderationDistance += returnDistance;

                #endregion

                #region Compare with Current Minimum

                if (totalConsiderationDistance < minimumUpperBound)
                    minimumUpperBound = totalConsiderationDistance;

                #endregion

            }

            return minimumUpperBound;

        }

    }
}
