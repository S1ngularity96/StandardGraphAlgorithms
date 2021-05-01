using System;
using System.Collections.Generic;
using MA.Interfaces;
using MA.Collections;
using MA.Classes;
using MA.Helper;
using MA.Exceptions;
using Priority_Queue;
namespace MA
{
    public static class Algorithms
    {
        [Flags]
        public enum MST
        {
            KRUSKAL = 0,
            PRIM = 1
        }
        public enum TSP
        {
            NEARESTNEIGHBOR = 0,
            DOUBLETREE = 1,
            ALLTOURS = 2,
            BRANCHANDBOUND = 3
        }
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
            if (GC.TryStartNoGCRegion(1024 * 1024 * 64))
            {
                try
                {

                    //Prepare Graph for Prim
                    int NUMBER_OF_NODES = g.NUMBER_OF_NODES();
                    Graph G_neu = new UndirectedGraph();
                    float Capacity_Sum = 0.0f;
                    g.UnmarkAllNodes();

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
                            Node node = g.nodes[edge.V_TO];
                            node.mark();
                            if (creategraph)
                                G_neu.AddEdge(edge.V_FROM, edge.V_TO, edge.GetCapacity());

                            Capacity_Sum = Capacity_Sum + edge.GetCapacity();
                            foreach (Edge neigbour_edge in node.edges)
                            {
                                if (!g.nodes[neigbour_edge.V_TO].isMarked())
                                {
                                    pr_queue.Enqueue(neigbour_edge, neigbour_edge.GetCapacity());
                                }
                            }
                        }
                    }

                    return new Tuple<float, Graph>(Capacity_Sum, G_neu);
                }
                finally
                {
                    GC.EndNoGCRegion();
                }
            }
            else
            {
                throw new Exception("Could not allocate enough RAM");
            }
        }

        public static Tuple<float, Graph> Kruskal(Graph g, bool creategraph)
        {
            //Prepare Graph for Kruskal
            int NUMBER_OF_NODES = g.NUMBER_OF_NODES();
            Graph G_neu = new UndirectedGraph();
            float Capacity_Sum = 0.0f;

            if (GC.TryStartNoGCRegion(1024 * 1024 * 64))
            {
                try
                {
                    if (creategraph)
                        G_neu.nodes = new Collections.NodeSet(NUMBER_OF_NODES);

                    SimplePriorityQueue<Edge> pr_queue = new SimplePriorityQueue<Edge>();
                    DisjointSetCollection set_collection = new DisjointSetCollection(NUMBER_OF_NODES);
                    //Put all Edges in Priority Queue
                    Diagnostic.MeasureTime(() =>
                    {
#if debug
                        System.Console.WriteLine("Kruskal: Adding adges to PriorityQueue");
#endif
                        foreach (Node node in g.nodes.Values)
                        {
                            foreach (Edge edge in node.edges)
                            {
                                pr_queue.Enqueue(edge, edge.GetCapacity());
                            }
                        }
                    });
                    Diagnostic.MeasureTime(() =>
                    {
#if debug
                        System.Console.WriteLine("Kruskal: Running through Edges in PriorityQueue");
#endif
                        //Run Kruskal Main-Algorithm-Part
                        while (pr_queue.Count != 0 && set_collection.NUMBER_OF_SETS() > 1)
                        {
                            Edge edge = pr_queue.Dequeue();
                            if (set_collection.union(edge.V_FROM, edge.V_TO))
                            {
                                Capacity_Sum += edge.GetCapacity();
                                if (creategraph)
                                    G_neu.AddEdge(edge.V_FROM, edge.V_TO, edge.GetCapacity());
                            }
                        }
                    });
                }
                finally
                {
                    GC.EndNoGCRegion();
                }
            }
            else
            {
                throw new Exception("Could not allocate enough RAM");
            }

            return new Tuple<float, Graph>(Capacity_Sum, G_neu);
        }

        ///<summary></summary>
        ///<returns>Tuple with the Tourlist as the first element and the costs as the second element</returns>
        public static Tuple<List<Node>, float> NearestNeighbor(Graph g, int NODE_S)
        {
            List<Node> Tour = new List<Node>();
            float tourCosts = 0.0f;

            int NUMBER_OF_NODES = g.NUMBER_OF_NODES();
            Node currentNode = g.nodes[NODE_S];
            currentNode.mark();
            Tour.Add(currentNode);
            int visited = 1;

            while (visited != NUMBER_OF_NODES)
            {
                Node neighbour = null;
                float cost = float.PositiveInfinity;
                foreach (Edge edge in currentNode.edges)
                {
                    if (!g.nodes[edge.V_TO].isMarked() && edge.GetCapacity() <= cost)
                    {
                        neighbour = g.nodes[edge.V_TO];
                        cost = edge.GetCapacity();
                    }
                }
                neighbour.mark();
                tourCosts += cost;
                Tour.Add(neighbour);
                currentNode = neighbour;
                visited++;
            }
            //Close Circle
            foreach (Edge edge in currentNode.edges)
            {
                if (edge.V_TO == NODE_S)
                {
                    System.Console.WriteLine(edge.V_TO);
                    tourCosts += edge.GetCapacity();
                    Tour.Add(g.nodes[NODE_S]);
                    return new Tuple<List<Node>, float>(Tour, tourCosts);
                }
            }
            throw new GraphException("NearestNeighbor: Could not find a Tour!");
        }


        public static Tuple<List<Node>, float> DoubleTree(Graph g, int NODE_S, MST choice)
        {
            List<Node> Tour = new List<Node>();
            float tourCosts = 0.0f;
            Graph MST_G = null;
            switch (choice)
            {
                case MST.KRUSKAL:
                    System.Console.WriteLine("Using MST-Kruskal");
                    MST_G = Kruskal(g, creategraph: true).Item2;
                    break;
                case MST.PRIM:
                    System.Console.WriteLine("Using MST-Prim");
                    MST_G = Prim(g, creategraph: true).Item2;
                    break;
                default:
                    throw new GraphException("Choose between Prim or Kruskal");
            }

            //Construct Euler Graph
            Node node = MST_G.nodes[NODE_S];
            node.mark();

            List<Edge>.Enumerator enumerator = node.edges.GetEnumerator();
            Stack<List<Edge>.Enumerator> stack = new Stack<List<Edge>.Enumerator>();
            Queue<Edge> edgeQueue = new Queue<Edge>();
            stack.Push(enumerator);

            //Create Euler-Circle (Same as DepthTraverse + collecting edges into a queue)
            while (stack.Count != 0)
            {
                var edge_loop_enumerator = stack.Pop();

                if (edge_loop_enumerator.Current != null)
                {
#if debug
                    System.Console.WriteLine($"b: {edge_loop_enumerator.Current.V_TO} -> {edge_loop_enumerator.Current.V_FROM}");
#endif
                    edgeQueue.Enqueue(new Edge(
                        edge_loop_enumerator.Current.V_TO,
                        edge_loop_enumerator.Current.V_FROM,
                        edge_loop_enumerator.Current.GetCapacity()));
                }
                while (edge_loop_enumerator.MoveNext())
                {
                    if (!MST_G.nodes[edge_loop_enumerator.Current.V_TO].isMarked())
                    {
                        MST_G.nodes[edge_loop_enumerator.Current.V_TO].mark();
#if debug
                        System.Console.WriteLine($"f: {edge_loop_enumerator.Current.V_FROM} -> {edge_loop_enumerator.Current.V_TO}");
#endif
                        edgeQueue.Enqueue(new Edge(
                            edge_loop_enumerator.Current.V_FROM,
                            edge_loop_enumerator.Current.V_TO,
                            edge_loop_enumerator.Current.GetCapacity()));
                        stack.Push(edge_loop_enumerator);
                        stack.Push(MST_G.nodes[edge_loop_enumerator.Current.V_TO].edges.GetEnumerator());
                        break;
                    }
                }
            }

            g.UnmarkAllNodes();
            while (edgeQueue.Count != 0)
            {
                Edge edge = edgeQueue.Dequeue();
                if (edgeQueue.Count == 0)
                {
                    if (edge.V_TO == NODE_S)
                    {
                        Tour.Add(g.nodes[edge.V_TO]);
                        tourCosts += edge.GetCapacity();
                    }
                    else
                    {
                        throw new GraphException("DoubleTree: Something went wrong. Could not find a Tour");
                    }
                }
                else
                {
                    if (!g.nodes[edge.V_FROM].isMarked() && !g.nodes[edge.V_TO].isMarked())
                    {
                        g.nodes[edge.V_FROM].mark();
                        g.nodes[edge.V_TO].mark();
                        Tour.Add(g.nodes[edge.V_FROM]);
                        Tour.Add(g.nodes[edge.V_TO]);
                        tourCosts += edge.GetCapacity();
                    }
                    else if (g.nodes[edge.V_FROM].isMarked() && !g.nodes[edge.V_TO].isMarked())
                    {
                        g.nodes[edge.V_TO].mark();
                        Tour.Add(g.nodes[edge.V_TO]);
                        tourCosts += edge.GetCapacity();
                    }
                    else if (g.nodes[edge.V_FROM].isMarked() && g.nodes[edge.V_TO].isMarked())
                    {
                        Node tmpNode = g.nodes[edge.V_FROM];
                        while (edgeQueue.Count > 1)
                        {
                            Edge nextEdge = edgeQueue.Dequeue();
                            if (!g.nodes[nextEdge.V_TO].isMarked())
                            {
                                Tour.Add(g.nodes[nextEdge.V_TO]);
                                g.nodes[nextEdge.V_TO].mark();

                                foreach (Edge tmpEdge in tmpNode.edges)
                                {
                                    if (tmpEdge.V_FROM == edge.V_FROM && nextEdge.V_TO == tmpEdge.V_TO)
                                    {
                                        tourCosts += tmpEdge.GetCapacity();
                                    }
                                }
                                break;
                            }
                        }
                    }

                }
            }

            return new Tuple<List<Node>, float>(Tour, tourCosts);
        }


    }
}