using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicGraphAdjacencyMatrices
{
    public static class DijkstraAlgorithm
    {

        private class NodeValues
        {

            public bool minimumDistanceSet = false;
            public float currentMinimumDistance = float.MaxValue;

            public bool SuggestNewMinimumDistance(float distance)
            {

                if (minimumDistanceSet)
                    return false;

                if (distance < currentMinimumDistance)
                {
                    currentMinimumDistance = distance;
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }

        /// <summary>
        /// Runs Dijkstra's algorithm on an adjacency matrix to find the shortest path form one node to another and also return the total distance of the path
        /// </summary>
        /// <param name="matrix">The matrix to use</param>
        /// <param name="startNode">The node to start at</param>
        /// <param name="endNode">The node to end at</param>
        /// <param name="totalDistance">The total distance of the route</param>
        /// <returns>The path as an array of nodes</returns>
        public static int[] Run(AdjacencyMatrix matrix,
            int startNode,
            int endNode,
            out float totalDistance)
        {

            #region Node values calculation

            NodeValues[] nodeValues = new NodeValues[matrix.NodeCount];

            for (int i = 0; i < nodeValues.Length; i++)
            {
                nodeValues[i] = new NodeValues();
            }

            nodeValues[startNode].currentMinimumDistance = 0;
            nodeValues[startNode].minimumDistanceSet = true;

            while (true)
            {
                
                if (nodeValues[endNode].minimumDistanceSet)
                    break;

                #region Setting updated working values

                for (int considerationStartNode = 0; considerationStartNode < matrix.NodeCount; considerationStartNode++)
                {

                    if (!nodeValues[considerationStartNode].minimumDistanceSet)
                        continue;

                    for (int considerationEndNode = 0; considerationEndNode < matrix.NodeCount; considerationEndNode++)
                    {

                        float? distance = matrix[considerationStartNode, considerationEndNode];

                        if (distance != null)
                            nodeValues[considerationEndNode].SuggestNewMinimumDistance(
                                nodeValues[considerationStartNode].currentMinimumDistance + (float)distance
                            );

                    }

                }

                #endregion

                #region Setting a final value

                float currentMinimumDistance = float.MaxValue;
                int currentMinimumDistanceNode = 0;

                for (int i = 0; i < matrix.NodeCount; i++)
                {

                    if (nodeValues[i].minimumDistanceSet)
                        continue;

                    if (nodeValues[i].currentMinimumDistance < currentMinimumDistance)
                    {
                        currentMinimumDistance = nodeValues[i].currentMinimumDistance;
                        currentMinimumDistanceNode = i;
                    }

                }

                nodeValues[currentMinimumDistanceNode].minimumDistanceSet = true;

                #endregion

                if (nodeValues[endNode].minimumDistanceSet)
                    break;

            }

            #endregion

            #region Path deduction

            List<int> nodePath = new List<int>() { endNode };

            while (true)
            {

                if (nodePath.Contains(startNode))
                {
                    break;
                }

                int prevNode = nodePath[0];

                foreach (int node in matrix.GetNodeSources(prevNode))
                {

                    float edgeLength = (float)matrix[node, prevNode];

                    if (!nodeValues[node].minimumDistanceSet)
                        continue;

                    if (nodeValues[node].currentMinimumDistance == nodeValues[prevNode].currentMinimumDistance - edgeLength)
                    {

                        nodePath.Insert(0, node);
                        break;

                    }

                }

            }

            #endregion

            totalDistance = nodeValues[endNode].currentMinimumDistance;
            return nodePath.ToArray();

        }

    }
}
