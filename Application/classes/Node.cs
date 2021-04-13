using System.Collections.Generic;
using System.Linq;
namespace MA.Classes
{
    public class Node
    {
        public int ID { get; set; }
        private bool V_MARKED { get; set; }
        List<Edge> edges = new List<Edge>();

        public Node() { }
        public Node(int id)
        {
            this.ID = id;
        }

        public void AddEdge(Node node)
        {
            edges.Add(new Edge(node));
        }

        public void AddEdge(Node node, float capacity)
        {
            edges.Add(new Edge(node, capacity));
        }

        public void RemoveEdge(int node, ref int NUMBER_OF_EDGES)
        {
            int SUM_EDGES = edges.Count;
            for (int edge = 0; edge < SUM_EDGES; edge++)
            {
                if (edges[edge].GetPointedNode().ID == node)
                {
                    edges.RemoveAt(edge);
                    NUMBER_OF_EDGES--;
                    return;
                }
            }
        }

        public List<Node> GetUnmarkedNeigbours()
        {
            List<Node> nodes = new List<Node>();
            foreach (Edge edge in edges)
            {
                if (!edge.GetPointedNode().isMarked())
                {
                    nodes.Add(edge.GetPointedNode());
                }
            }
            return nodes;
        }

        #region mark
        public bool isMarked()
        {
            return this.V_MARKED;
        }

        public void mark()
        {
            this.V_MARKED = true;
        }

        public void unmark()
        {
            this.V_MARKED = false;
        }
        #endregion


        public override string ToString()
        {
            return $"Node: {this.ID}";
        }

    }
}