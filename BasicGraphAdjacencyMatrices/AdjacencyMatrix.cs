﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicGraphAdjacencyMatrices
{

    public class AdjacencyMatrix
    {

        //Values of null mean no available route
        private double?[,] matrix;
        public int NodeCount => matrix.GetLength(0);

        public double?[,] GetMatrixCopy()
        {

            double?[,] output = new double?[NodeCount, NodeCount];

            for (int x = 0; x < NodeCount; x++)
                for (int y = 0; y < NodeCount; y++)
                    output[x, y] = this[x, y];

            return output;

        }

        public double? this[int startNode, int endNode]
        {
            get
            {
                return matrix[startNode, endNode];
            }
            set
            {
                matrix[startNode, endNode] = value;
            }
        }

        public AdjacencyMatrix(int size)
        {

            matrix = new double?[size,size];

        }

        public AdjacencyMatrix(double?[,] values)
        {

            SetValues(values);

        }

        public bool IsUndirected
        {
            get
            {

                for (int a = 0; a < NodeCount; a++)
                {
                    for (int b = a; b < NodeCount; b++) //Used "NodeCount - a" as some values will already have been checked
                    {

                        if (this[a, b] != this[b, a])
                        {
                            return false;
                        }

                    }
                }

                return true;

            }
        }

        public int GetNodeValency(int node)
        {

            if (!IsUndirected)
                throw new ArgumentException("Shouldn't be checking node valencies of directed graphs");

            int valency = 0;

            for (int otherNode = 0; otherNode < NodeCount; otherNode++)
                if (matrix[node, otherNode] != null)
                    valency++;

            return valency;

        }

        public double GetTotalValency()
        {

            if (!IsUndirected)
                throw new ArgumentException("Cannot find total valency of directed graph");

            double totalValency = 0;

            for (int column = 0; column < NodeCount; column++)
                for (int row = column + 1; row < NodeCount; row++)
                {

                    double? entry = matrix[column, row];

                    if (entry != null)
                        totalValency += (double)entry;

                }

            return totalValency;

        }

        public bool IsEulerian
        {
            get
            {

                if (!IsUndirected)
                    throw new ArgumentException("Shouldn't be checking whether graph is Eulerian using a directed graph");

                for (int node = 0; node < NodeCount; node++)
                {

                    if (GetNodeValency(node) % 2 != 0)
                        return false;

                }

                return true;

            }
        }

        public bool IsSemiEulerian
        {
            get
            {

                if (!IsUndirected)
                    throw new ArgumentException("Shouldn't be checking whether graph is semi-Eulerian using a directed graph");

                int nonEvenNodes = 0;

                for (int node = 0; node < NodeCount; node++)
                {

                    if (GetNodeValency(node) % 2 != 0)
                        nonEvenNodes++;

                }

                return nonEvenNodes == 2;

            }
        }

        public void SetValues(double?[,] values) => matrix = values;

        /// <summary>
        /// Removes the edge connecting the start and end nodes in one direction
        /// </summary>
        /// <param name="startNode">The starting node</param>
        /// <param name="endNode">The ending node</param>
        public void ClearEdge(int startNode,
            int endNode)
        {
            matrix[startNode, endNode] = null;
        }

        /// <summary>
        /// Removes the edge connecting the two nodes in both directions
        /// </summary>
        /// <param name="node1">The first node</param>
        /// <param name="node2">The second node</param>
        public void ClearEdgeUndirected(int node1,
            int node2)
        {
            ClearEdge(node1, node2);
            ClearEdge(node2, node1);
        }

        /// <summary>
        /// Sets the edge connecting the specified start and end nodes in one direction
        /// </summary>
        /// <param name="startNode">The start node</param>
        /// <param name="endNode">The end node</param>
        /// <param name="distance">The distance between the nodes</param>
        public void SetEdge(int startNode,
            int endNode,
            double distance)
        {
            matrix[startNode, endNode] = distance;
        }

        /// <summary>
        /// Sets the edge connecting the two nodes in both directions
        /// </summary>
        /// <param name="node1">The first node</param>
        /// <param name="node2">The second node</param>
        /// <param name="distance">The distance between the two nodes</param>
        public void SetEdgeUndirected(int node1,
            int node2,
            double distance)
        {
            SetEdge(node1, node2, distance);
            SetEdge(node2, node1, distance);
        }

        /// <summary>
        /// Gets all the nodes that the specified node has an edge to where the specified node is the source node
        /// </summary>
        /// <param name="node">The node to query</param>
        /// <returns>An array of nodes</returns>
        public int[] GetNodeDestinations(int node)
        {

            List<int> results = new List<int>();

            for (int i = 0; i < NodeCount; i++)
            {

                if (i == node)
                    continue;

                if (this[node, i] != null)
                {
                    results.Add(i);
                }

            }

            return results.ToArray();

        }

        /// <summary>
        /// Gets all the nodes that have an edge to the specified node where the specified node is the destination of the edge
        /// </summary>
        /// <param name="node">The node to query</param>
        /// <returns>An array of ndoes</returns>
        public int[] GetNodeSources(int node)
        {

            List<int> results = new List<int>();

            for (int i = 0; i < NodeCount; i++)
            {

                if (i == node)
                    continue;

                if (this[i, node] != null)
                {
                    results.Add(i);
                }

            }

            return results.ToArray();

        }

        public AdjacencyMatrix GenerateTableOfLeastDistances()
        {

            double?[,] newMatrixValues = new double?[NodeCount, NodeCount];

            for (int column = 0; column < NodeCount; column++)
            {
                for (int row = 0; row < NodeCount; row++)
                {

                    if (column == row)
                        newMatrixValues[column, row] = null;

                    DijkstraAlgorithm.Run(this, column, row, out double entryDistance);

                    newMatrixValues[column, row] = entryDistance;

                }
            }

            return new AdjacencyMatrix(newMatrixValues);

        }

    }

}
