using System.Collections.Generic;
namespace MA.Classes
{

    public class Node
    {
        public enum NodeType { SOURCE, SINK, NONE };
        public int ID { get; set; }
        private bool V_MARKED { get; set; }
        public float DISTANCE { get; set; }
        private float BALANCE { get; set;}
        private float R_BALANCE {get;set;}
        public Edge Predecessor = null;
        public List<Edge> edges = new List<Edge>();

        public NodeType type = NodeType.NONE;

        public Node() { }
        public Node(int id)
        {
            this.ID = id;
        }

        public void SetBalance(float balance)
        {
            if (balance > 0)
            {
                this.type = NodeType.SOURCE;
            }
            else if (balance < 0)
            {
                this.type = NodeType.SINK;
            }
            this.BALANCE = balance;
        }

        public float GetBalance(){
            return this.BALANCE;
        }

        public void SetR_Balance(float r_balance){
            this.R_BALANCE = r_balance;
        }

        public float GetR_Balance(){
            return this.R_BALANCE;
        }

        public void AddEdge(int V_FROM, int V_TO)
        {
            edges.Add(new Edge(V_FROM, V_TO));
        }

        public void AddEdge(int V_FROM, int V_TO, float capacity)
        {
            edges.Add(new Edge(V_FROM, V_TO, capacity));
        }

        public void AddEdge(Edge edge)
        {
            edges.Add(edge);
        }

#warning If is undirected Graph. Make also sure to remove same edge from other node.
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


        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(this.GetType()))
            {
                Node other = (Node)obj;
                if (other.ID == this.ID)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return $"Node: {this.ID}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}