using MA.Interfaces;
using MA.Classes;
using System.Collections.Generic;
using Priority_Queue;
namespace MA
{
    public static class GraphUtils
    {
        #region Structs
        public struct SPValues
        {
            public Graph G_neu;
            public SimplePriorityQueue<int> VQueue;
            public float? DISTANCE;
        }
        #endregion

        public static SPValues InitSP(Graph g, int NODE_S, Algorithms.SP choice)
        {
            SPValues sPValues = new SPValues();
            sPValues.VQueue = new SimplePriorityQueue<int>();
            sPValues.G_neu = new DirectedGraph();
            foreach (Node node in g.nodes.Values)
            {
                node.DISTANCE = float.PositiveInfinity;
                node.unmark();
                sPValues.VQueue.Enqueue(node.ID, node.DISTANCE);

            }
            if (Algorithms.SP.DIJKSTRA == choice)
            {
                g.nodes[NODE_S].DISTANCE = 0.0f;
                sPValues.VQueue.UpdatePriority(NODE_S, g.nodes[NODE_S].DISTANCE);
            }

            return sPValues;
        }

        public static List<Node> GetUnmarkedNeighbours(Graph g, int N)
        {
            List<Node> neighbours = new List<Node>();
            foreach (Edge edge in g.nodes[N].edges)
            {
                if (!g.nodes[edge.V_TO].isMarked())
                {
                    neighbours.Add(g.nodes[edge.V_TO]);
                }
            }
            return neighbours;
        }

        public static void PrintTourWithCosts(List<Node> tour, float cost)
        {
            List<Node> copy = new List<Node>(tour);
            System.Console.WriteLine($"Tour with the cost of {cost} and {copy.Count} stations");
            System.Console.Write("{ ");

            while (copy.Count > 1)
            {
                System.Console.Write($"{copy[0].ID} -> ");
                copy.RemoveAt(0);
            }
            if (copy.Count == 1)
            {
                System.Console.Write($"{copy[0].ID}");
            }
            System.Console.WriteLine(" }");
        }

    }
}