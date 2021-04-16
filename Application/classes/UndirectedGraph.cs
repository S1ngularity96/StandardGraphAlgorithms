
using MA.Interfaces;
using MA.Exceptions;
using MA.Collections;
using System;
using System.IO;
namespace MA.Classes
{
    public class UndirectedGraph : Graph
    {

        public override void AddEdge(int N1, int N2, float capacity)
        {
            if (this.nodes == null)
            {
                throw new GraphException("No nodes in Graph exist");
            }
            Node n1 = nodes[N1];
            Node n2 = nodes[N2];
            n1.AddEdge(N1, N2, capacity);
            n2.AddEdge(N2, N1, capacity);
            NUMBER_OF_EDGES++;
        }

        public override string ToString()
        {
            return $"Undirected Graph\n|V| = {nodes.Count}\n|E| = {NUMBER_OF_EDGES}";
        }
    }
}