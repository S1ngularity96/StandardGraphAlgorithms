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
                g.MarkNode(node.ID);
                queue.Enqueue(node);
                // BreadthSearch itself
                while (queue.Count > 0)
                {
                    Node firstNode = queue.Dequeue();
                    List<Node> neighbours = firstNode.GetUnmarkedNeigbours();

                    foreach (Node neighbour in neighbours)
                    {
                        neighbour.mark();
                        g.MarkNode(neighbour.ID);
                        queue.Enqueue(neighbour);
                    }
                }
                components++;
            }
            return components;
        }

        public static int DepthSearch(Graph g)
        {
            int components = 0;
            System.Console.WriteLine("Counting Graph-Components ...");
            g.UnmarkAllNodes();
            for (Node node = g.GetFirstUnmarkedNode(); node != null; node = g.GetFirstUnmarkedNode())
            {
                components++;
                DepthTraverse(g, node);
            }
            return components;
        }

        public static void DepthTraverse(Graph g, Node node)
        {
            if (node == null) { return; }
            node.mark();
            g.MarkNode(node.ID);

            List<Edge>.Enumerator enumerator = node.edges.GetEnumerator();
            Stack<List<Edge>.Enumerator> stack = new Stack<List<Edge>.Enumerator>();
            stack.Push(enumerator);

            while (stack.Count != 0)
            {
                var edge_loop_enumerator = stack.Pop();
                while (edge_loop_enumerator.MoveNext())
                {
                    if (!edge_loop_enumerator.Current.GetPointedNode().isMarked())
                    {
                        edge_loop_enumerator.Current.GetPointedNode().mark();
                        g.MarkNode(edge_loop_enumerator.Current.GetPointedNode().ID);
                        stack.Push(edge_loop_enumerator);
                        stack.Push(edge_loop_enumerator.Current.GetPointedNode().edges.GetEnumerator());
                        break;
                    }
                }
            }

        }
    }
}