using System.Collections.Generic;
namespace MA.Classes
{
    public class Node
    {
        int ID { get; set; }
        private bool V_MARKED { get; set; }
        List<Edge> edges = new List<Edge>();
        public void AddEdge(int node)
        {
            edges.Add(new Edge(node));
        }

        public void AddEdge(int node, float capacity)
        {
            edges.Add(new Edge(node, capacity));
        }

        public void RemoveEdge(int node, ref int NUMBER_OF_EDGES)
        {
            int SUM_EDGES = edges.Count;
            for (int edge = 0; edge < SUM_EDGES; edge++)
            {
                if (edges[edge].GetPointedNodeID() == node)
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