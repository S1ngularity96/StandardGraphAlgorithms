using MA.Interfaces;
using System.Collections.Generic;
using MA.Classes;
using System;
using Priority_Queue;
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

        public static Tuple<float, Graph> Prim(Graph g, bool creategraph)
        {
            //Prepare Graph for Prim
            int NUMBER_OF_NODES = g.NUMBER_OF_NODES();
            Graph G_neu = new UndirectedGraph();
            float Capacity_Sum = 0.0f;
            g.UnmarkAllNodes();
            HashSet<Edge> visited_edges = new HashSet<Edge>();

            if (creategraph)
                G_neu.nodes = new Collections.NodeSet(NUMBER_OF_NODES);
            //Prepare Queue
            Node node_random = g.GetFirstUnmarkedNode();
            node_random.mark();
            SimplePriorityQueue<Edge> pr_queue = new SimplePriorityQueue<Edge>();

            foreach (Edge edge in node_random.edges)
            {
                pr_queue.Enqueue(edge, edge.GetCapacity());
            }

            //Iterations

            while (pr_queue.Count != 0)
            {
                Edge edge = pr_queue.Dequeue();
                if (!g.nodes[edge.V_TO].isMarked())
                {
                    visited_edges.Add(edge);
                    Node node = g.nodes[edge.V_TO];
                    node.mark();
                    if (creategraph)
                        G_neu.AddEdge(edge.V_FROM, edge.V_TO, edge.GetCapacity());

                    Capacity_Sum = Capacity_Sum + edge.GetCapacity();
                    foreach (Edge neigbour_edge in node.edges)
                    {
                        if (!visited_edges.Contains(neigbour_edge))
                        {
                            pr_queue.Enqueue(neigbour_edge, neigbour_edge.GetCapacity());
                        }
                    }
                }
            }
            return new Tuple<float, Graph>(Capacity_Sum, G_neu);
        }

        public static void Kruskal(Graph g)
        {

        }

    }
}