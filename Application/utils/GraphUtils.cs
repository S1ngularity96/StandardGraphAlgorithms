using MA.Interfaces;
using MA.Classes;
using System.Collections.Generic;
namespace MA
{
    public static class GraphUtils
    {

        public static List<Node> GetUnmarkedNeighbours(Graph g, int N)
        {
            List<Node> neighbours = new List<Node>();
            foreach (Edge edge in g.nodes[N].edges)
            {
                if (!g.nodes[edge.V_TO].isMarked())
                {
                    neighbours.Add(g.nodes[edge.V_TO]);
                }
            }
            return neighbours;
        }


    }
}