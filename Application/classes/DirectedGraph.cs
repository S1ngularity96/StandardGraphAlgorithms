using MA.Interfaces;
using MA.Exceptions;
namespace MA.Classes
{
    public class DirectedGraph : Graph
    {

        public override void AddEdge(int N1, int N2, float capacity)
        {
            if (this.nodes == null)
            {
                throw new GraphException("No nodes in Graph exist");
            }
            Node n1 = nodes.GetOrAdd(N1);
            n1.AddEdge(N1, N2, capacity);

        }

        public override int NUMBER_OF_EDGES()
        {
            int countedEdges = 0;
            if (this.nodes != null)
            {
                foreach (Node n in this.nodes.Values)
                {
                    countedEdges += n.edges.Count;
                }
            }

            return countedEdges;
        }

        public override string ToString()
        {
            return $"Directed Graph\n|V| = {nodes.Count}\n|E| = {NUMBER_OF_EDGES()}";
        }


    }
}