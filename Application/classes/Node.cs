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