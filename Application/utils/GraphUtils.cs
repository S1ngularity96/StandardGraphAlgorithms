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

        public static void PrintTourWithCosts(List<Node> tour, float cost)
        {
            List<Node> copy = new List<Node>(tour);
            System.Console.WriteLine($"Tour with the cost of {cost}");
            System.Console.Write("{ ");

            while (copy.Count > 1)
            {
                System.Console.Write($"{copy[0].ID} -> ");
                copy.RemoveAt(0);
            }
            if (copy.Count == 1)
            {
                System.Console.Write($"{copy[0].ID}");
            }
            System.Console.WriteLine(" }");
        }


    }
}