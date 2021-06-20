using MA.Interfaces;
using MA.Classes;
using MA.Exceptions;
using System.Collections.Generic;
using Priority_Queue;
namespace MA
{
    public static class GraphUtils
    {
        #region Structs
        public struct BFSPResult
        {
            public Graph G_neu;
            public List<Edge> edges;
            public float? DISTANCE;
            public Edge negativeCycleEdge;
        }

        public struct DSPResult
        {
            public Graph G_neu;
            public SimplePriorityQueue<int> VQueue;
            public float? DISTANCE;
        }
        #endregion

        public static DSPResult InitDSP(Graph g, int NODE_S)
        {
            DSPResult result = new DSPResult();
            result.G_neu = new DirectedGraph();
            result.VQueue = new SimplePriorityQueue<int>();

            foreach (Node node in g.nodes.Values)
            {
                node.DISTANCE = float.PositiveInfinity;
                node.unmark();
                result.VQueue.Enqueue(node.ID, node.DISTANCE);
            }
            g.nodes[NODE_S].DISTANCE = 0.0f;
            result.VQueue.UpdatePriority(NODE_S, g.nodes[NODE_S].DISTANCE);

            return result;
        }

        public static BFSPResult InitBFSP(Graph g, int NODE_S)
        {
            BFSPResult result = new BFSPResult();
            result.G_neu = new DirectedGraph();
            result.edges = new List<Edge>(g.NUMBER_OF_EDGES());
            foreach (Node node in g.nodes.Values)
            {
                node.DISTANCE = float.PositiveInfinity;
                node.unmark();
                result.edges.AddRange(node.edges);

            }
            g.nodes[NODE_S].DISTANCE = 0.0f;
            return result;
        }

        public static List<int> GetNodeIdsOfType(Graph g, Node.NodeType type){
            List<int> ids = new List<int>();
            foreach(Node node in g.nodes.Values){
                if(node.type == type){
                    ids.Add(node.ID);
                }
            }
            return ids;
        }

        public static Edge GetEdgeFromTo(Graph g, int V_FROM, int V_TO){
            if(g.nodes.ContainsKey(V_FROM)){
                foreach(Edge edge in g.nodes[V_FROM].edges){
                    if(edge.V_TO == V_TO){
                        return edge;
                    }
                }
                throw new GraphException($"Could not find V_TO ({V_FROM} -> {V_TO})");
            }
            throw new GraphException($"Could not find V_FROM ({V_FROM} -> {V_TO})");
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