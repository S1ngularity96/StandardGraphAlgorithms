using MA.Interfaces;
using System.Collections.Generic;
using MA.Classes;
using System;
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
    }
}