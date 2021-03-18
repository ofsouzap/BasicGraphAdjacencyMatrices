using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicGraphAdjacencyMatrices
{

    public static class RouteInspection
    {

        public static double FindShortestRouteLength(AdjacencyMatrix matrix) => FindShortestRouteLength(matrix, null, null);

        /// <summary>
        /// Finds the length of the shortest path using every edge at least once an returning to the starting node using a provided undirected adjacency matrix
        /// </summary>
        /// <param name="matrix">The matrix to use</param>
        /// <param name="routeTable">A route table for the matrix made using Floyd's algorithm</param>
        /// <param name="distanceTable">A distance table for the matrix made using Floyd's algorithm</param>
        /// <returns>The shortest path length</returns>
        public static double FindShortestRouteLength(AdjacencyMatrix matrix,
            int[,] routeTable,
            double[,] distanceTable)
        {

            if (!matrix.IsUndirected)
            {
                throw new ArgumentException("Cannot find shortest route length for directed graphs");
            }

            if (matrix.IsEulerian)
                return FindEulerianGraphShortestRouteLength(matrix);
            else if (matrix.IsSemiEulerian)
                return FindSemiEulerianGraphShortestRouteLength(matrix);
            else
                return RunAlgorithm(matrix, routeTable, distanceTable);

        }

        private static double FindEulerianGraphShortestRouteLength(AdjacencyMatrix matrix)
            => matrix.GetTotalValency();

        private static double FindSemiEulerianGraphShortestRouteLength(AdjacencyMatrix matrix)
        {

            int oddNode1, oddNode2;

            int[] oddNodes = GetOddNodes(matrix);

            if (oddNodes.Length != 2)
                throw new ArgumentException("Provided graph isn't semi-Eulerian");

            oddNode1 = oddNodes[0];
            oddNode2 = oddNodes[1];

            DijkstraAlgorithm.Run(matrix, oddNode1, oddNode2, out double oddNodeShortestDistance);

            return matrix.GetTotalValency() + oddNodeShortestDistance;

        }

        private static double RunAlgorithm(AdjacencyMatrix matrix) => RunAlgorithm(matrix, null, null);

        private static double RunAlgorithm(AdjacencyMatrix matrix,
            int[,] matrixRouteTable,
            double[,] matrixDistanceTable)
        {

            int[] oddNodes = GetOddNodes(matrix);

            if (matrixRouteTable == null || matrixDistanceTable == null)
                matrixRouteTable = FloydAlgorithm.Run(matrix, out matrixDistanceTable);

            int[][][] nodePairings = GetNodePairings(oddNodes);

            FindNodePairingOfLeastTotalLength(
                nodePairings,
                matrixRouteTable,
                matrixDistanceTable,
                out double leastPairingLength
                );

            return matrix.GetTotalValency() + leastPairingLength;

        }

        private static int[][] FindNodePairingOfLeastTotalLength(int[][][] pairings,
            int[,] routeTable,
            double[,] distanceTable,
            out double leastTotalLength)
        {

            int[][] minimumLengthPairing = new int[0][];
            double leastLength = double.MaxValue;

            foreach (int[][] pairing in pairings)
            {

                double pairingLength = FindPairingLength(pairing,
                    routeTable,
                    distanceTable);

                if (pairingLength < leastLength)
                {
                    minimumLengthPairing = pairing;
                    leastLength = pairingLength;
                }

            }

            leastTotalLength = leastLength;
            return minimumLengthPairing;

        }

        private static double FindPairingLength(int[][] pairing,
            int[,] routeTable,
            double[,] distanceTable)
        {

            double totalDistance = 0;

            foreach (int[] pair in pairing)
            {
                FloydAlgorithm.FindRoute(pair[0],
                    pair[1],
                    routeTable,
                    distanceTable,
                    out double distance);
                totalDistance += distance;
            }

            return totalDistance;

        }

        /// <summary>
        /// Finds all the possible pairings of the provided array of nodes
        /// </summary>
        /// <param name="nodes">The nodes to use</param>
        /// <returns>An array listing each possible combination of pairings. Each combination is an array of pairs. Each pair is a 2-element array containing the paired nodes</returns>
        private static int[][][] GetNodePairings(int[] nodes)
        {

            // This method of finding permutations and using them to form pairings isn't very
            //     efficient but I can't think of another way of solving the problem

            List<int[][]> pairingsList = new List<int[][]>(
                GetNodePermutations(nodes)
                .Select(x => NodePermutationToPairings(x))
                .ToArray()
            );

            while (true)
            {

                bool listChanged = false;

                for (int pairingIndex1 = 0; pairingIndex1 < pairingsList.Count; pairingIndex1++)
                {
                    for (int pairingIndex2 = pairingIndex1 + 1; pairingIndex2 < pairingsList.Count; pairingIndex2++)
                    {

                        bool nonEquivalantPairFound = false;

                        for (int pairIndex1 = 0; pairIndex1 < pairingsList[pairingIndex1].Length; pairIndex1++)
                        {

                            for (int pairIndex2 = pairIndex1 + 1; pairIndex2 < pairingsList[pairingIndex1].Length; pairIndex2++)
                                if (CompareNodePairing(
                                    pairingsList[pairingIndex1][pairIndex1],
                                    pairingsList[pairingIndex2][pairIndex2]
                                    ))
                                {
                                    nonEquivalantPairFound = true;
                                    break;
                                }

                            if (nonEquivalantPairFound)
                                break;

                        }

                        if (nonEquivalantPairFound)
                        {
                            pairingsList.RemoveAt(pairingIndex2);
                            listChanged = true;
                            break;
                        }

                    }

                    if (listChanged)
                        break;

                }

                if (!listChanged)
                    break;

            }

            return pairingsList.ToArray();

        }

        private static bool CompareNodePairing(int[] nodePairing1, int[] nodePairing2)
        {
            return (nodePairing1[0] == nodePairing2[0]
                    && nodePairing1[1] == nodePairing2[1])
                || (nodePairing1[0] == nodePairing2[1]
                    && nodePairing1[1] == nodePairing2[0]);
        }

        private static int[][] NodePermutationToPairings(int[] permutation)
        {

            if (permutation.Length % 2 != 0)
                throw new ArgumentException("Permutation array length not even");

            List<int[]> pairings = new List<int[]>();

            for (int i = 0; i < permutation.Length - 1; i += 2)
                pairings.Add(new int[] { permutation[i], permutation[i + 1] });

            return pairings.ToArray();

        }

        private static int[][] GetNodePermutations(int[] nodes)
        {

            if (nodes.Length == 1)
            {
                return new int[][] { new int[] { nodes[0] } };
            }
            else
            {

                List<int[]> permutations = new List<int[]>();

                for (int i = 0; i < nodes.Length; i++)
                {

                    int endingOptionsIndex = 0;
                    int[] endingOptions = new int[nodes.Length - 1];

                    foreach (int node in nodes)
                    {

                        if (node == nodes[i])
                            continue;

                        endingOptions[endingOptionsIndex] = node;
                        endingOptionsIndex++;

                    }

                    int[][] nodeEndings = GetNodePermutations(endingOptions);

                    foreach (int[] ending in nodeEndings)
                    {

                        List<int> newPermutation = new List<int> { nodes[i] };

                        newPermutation.AddRange(ending);

                        permutations.Add(newPermutation.ToArray());

                    }

                }

                return permutations.ToArray();

            }

        }

        private static int[] GetOddNodes(AdjacencyMatrix matrix)
        {

            List<int> oddNodes = new List<int>();

            for (int node = 0; node < matrix.NodeCount; node++)
                if (matrix.GetNodeValency(node) % 2 != 0)
                    oddNodes.Add(node);

            return oddNodes.ToArray();

        }

    }

}
