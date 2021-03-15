using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicGraphAdjacencyMatrices
{
    public static class PrimAlgorithm
    {

        /// <summary>
        /// Runs Prim's Algorithm on a provided adjacency matrix to find a Minimum Spanning Tree for it and returning the edges that are used. This only works for an undirected graph's matrix
        /// </summary>
        /// <param name="matrix">The matrix to consider</param>
        /// <returns>An array of pairs of integers representing the edges that are included in the MST. For example, returning [ [0,1] , [3,0] , [2,1] ] means that the edges 0-1, 3-0 and 2-1 should be included in the MST</returns>
        public static int[][] Run(AdjacencyMatrix matrix)
        {

            if (!matrix.IsUndirected)
            {
                throw new ArgumentException("Provided matrix was directed");
            }

            List<int[]> currentTree = new List<int[]>();

            bool[] usedNodes = new bool[matrix.NodeCount]; //Each value will default to false
            usedNodes[0] = true; //Set starting node

            while (true)
            {

                int minimumDistanceNewNode = 0; //The node that might be set to used
                int[] minimumDistanceArc = null;
                float minimumDistance = float.MaxValue;

                #region Find minimum edge

                for (int fromNode = 0; fromNode < matrix.NodeCount; fromNode++)
                {

                    if (!usedNodes[fromNode]) //Only look at nodes in consideration
                        continue;

                    for (int toNode = 0; toNode < matrix.NodeCount; toNode++)
                    {

                        if (usedNodes[toNode]) //Don't use node if already considered
                            continue;

                        float? entry = matrix[fromNode, toNode];

                        if (entry == null)
                            continue;
                        else
                        {

                            float distance = (float)entry;

                            if (distance < minimumDistance)
                            {
                                minimumDistance = distance;
                                minimumDistanceArc = new int[] { fromNode, toNode };
                                minimumDistanceNewNode = toNode;
                            }

                        }

                    }

                }

                #endregion

                #region Commit minimum edge

                currentTree.Add(minimumDistanceArc);
                usedNodes[minimumDistanceNewNode] = true;

                #endregion

                #region Check if complete

                if (usedNodes.All(x => x))
                {
                    break;
                }

                #endregion

            }

            return currentTree.ToArray();

        }

    }
}
