using MA.Interfaces;
using MA.Collections;
using System.IO;
namespace MA.Classes
{
    public class DirectedGraph : Graph
    {

        public override void AddEdge(int N1, int N2, float capacity)
        {
            Node node = nodes.GetOrAdd(N1);
            node.AddEdge(N2, capacity);
            this.NUMBER_OF_EDGES++;
        }

        public override string ToString()
        {
            return $"Directed Graph\n|V| = {nodes.Count}\n|E| = {NUMBER_OF_EDGES}";
        }
    }
}