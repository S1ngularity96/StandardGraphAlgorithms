using MA.Interfaces;
using System.Collections.Generic;
using MA.Classes;
using MA.Collections;
namespace MA
{
    public static class Algorithms
    {
        public static int BreadthSearch(Graph g)
        {
            System.Console.WriteLine("Counting Graph-Components ...");
            g.UnmarkAllNodes();
            int components = 0;
            Queue<Node> queue = new Queue<Node>();
            // Foreach Graph-Component
            for (Node node = g.GetFirstUnmarkedNode(); node != null; node = g.GetFirstUnmarkedNode())
            {
                node.mark();
                queue.Enqueue(node);
                // BreadthSearch itself
                while (queue.Count > 0)
                {
                    Node firstNode = queue.Dequeue();
                    List<Node> neighbours = firstNode.GetUnmarkedNeigbours();

                    foreach (Node neighbour in neighbours)
                    {
                        neighbour.mark();
                        queue.Enqueue(neighbour);
                    }
                }
                components++;
            }
            return components;
        }

        public static int DepthSearch(Graph g)
        {
            System.Console.WriteLine("Counting Graph-Components ... ");
            int components = 0;
            g.UnmarkAllNodes();
            Stack<Node> stack = new Stack<Node>();
            for (Node node = g.GetFirstUnmarkedNode(); node != null; node = g.GetFirstUnmarkedNode())
            {
                node.mark();
                stack.Push(node);
                while (stack.Count != 0)
                {
                    Node topNode = stack.Pop();
                    foreach (Edge edge in topNode.edges)
                    {
                        Node pointed = edge.GetPointedNode();
                        if (!pointed.isMarked())
                        {
                            pointed.mark();
                            stack.Push(pointed);
                        }

                    }
                }
                components++;
            }

            return components;
        }
    }
}