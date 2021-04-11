using MA.Interfaces;
using System;
namespace MA.Classes
{
    public class DirectedGraph : Graph
    {
        public override void AddEdge(int N1, int N2)
        {
            Node node = nodes.GetOrAdd(N1);
            node.AddEdge(N2);
            this.NUMBER_OF_EDGES++;
        }

        public override void AddEdge(int N1, int N2, float capacity)
        {
            throw new NotImplementedException();
        }

        public override void ReadFromFile(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}