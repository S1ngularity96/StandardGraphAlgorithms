using MA.Interfaces;
using System.Collections.Generic;
using MA.Classes;
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
            int NUMBER_OF_NODES = g.NUMBER_OF_NODES();
            for (int current_node = 0; current_node < NUMBER_OF_NODES; current_node++)
            {
                Node node = g.nodes[current_node];
                if (!node.isMarked())
                {
                    node.mark();
                    queue.Enqueue(node);
                    // BreadthSearch itself
                    while (queue.Count > 0)
                    {
                        Node firstNode = queue.Dequeue();
                        List<Node> neighbours = GraphUtils.GetUnmarkedNeighbours(g, firstNode.ID);

                        foreach (Node neighbour in neighbours)
                        {
                            neighbour.mark();
                            queue.Enqueue(neighbour);
                        }
                    }
                    components++;
                }

            }
            return components;
        }

        public static int DepthSearch(Graph g)
        {
            int components = 0;
            System.Console.WriteLine("Counting Graph-Components ...");
            g.UnmarkAllNodes();
            int NUMBER_OF_NODES = g.NUMBER_OF_NODES();
            for (int current_node = 0; current_node < NUMBER_OF_NODES; current_node++)
            {
                Node node = g.nodes[current_node];
                if (!node.isMarked())
                {
                    components++;
                    DepthTraverse(g, node);
                }
            }
            return components;
        }

        public static void DepthTraverse(Graph g, Node node)
        {
            if (node == null) { return; }
            node.mark();

            List<Edge>.Enumerator enumerator = node.edges.GetEnumerator();
            Stack<List<Edge>.Enumerator> stack = new Stack<List<Edge>.Enumerator>();
            stack.Push(enumerator);

            while (stack.Count != 0)
            {
                var edge_loop_enumerator = stack.Pop();
                while (edge_loop_enumerator.MoveNext())
                {
                    if (!g.nodes[edge_loop_enumerator.Current.V_TO].isMarked())
                    {
                        g.nodes[edge_loop_enumerator.Current.V_TO].mark();
                        stack.Push(edge_loop_enumerator);
                        stack.Push(g.nodes[edge_loop_enumerator.Current.V_TO].edges.GetEnumerator());
                        break;
                    }
                }
            }

        }

        public static void Prim(Graph g)
        {

        }

        public static void Kruskal(Graph g)
        {

        }

    }
}