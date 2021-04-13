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
            Node n1 = nodes[N1];
            Node n2 = nodes[N2];
            n1.AddEdge(n2, capacity);
            NUMBER_OF_EDGES++;
        }

        public override string ToString()
        {
            return $"Directed Graph\n|V| = {nodes.Count}\n|E| = {NUMBER_OF_EDGES}";
        }
    }
}