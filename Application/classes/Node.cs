using System.Collections.Generic;
namespace MA.Classes
{
    public class Node
    {
        public int ID { get; set; }
        private bool V_MARKED { get; set; }
        public List<Edge> edges = new List<Edge>();

        public Node() { }
        public Node(int id)
        {
            this.ID = id;
        }

        public void AddEdge(int V_FROM, int V_TO)
        {
            edges.Add(new Edge(V_FROM, V_TO));
        }

        public void AddEdge(int V_FROM, int V_TO, float capacity)
        {
            edges.Add(new Edge(V_FROM, V_TO, capacity));
        }

#warning Will not work with undirected edges
        public void RemoveEdge(int node, ref int NUMBER_OF_EDGES)
        {
            int SUM_EDGES = edges.Count;
            for (int edge = 0; edge < SUM_EDGES; edge++)
            {
                if (edges[edge].V_TO == node)
                {
                    edges.RemoveAt(edge);
                    NUMBER_OF_EDGES--;
                    return;
                }
            }
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