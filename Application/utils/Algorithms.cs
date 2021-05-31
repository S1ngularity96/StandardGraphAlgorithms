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
        [Flags]
        public enum TSP
        {
            NEARESTNEIGHBOR = 0,
            DOUBLETREE = 1,
            ALLTOURS = 2,
            BRANCHANDBOUND = 3
        }
        [Flags]
        public enum SP
        {
            DIJKSTRA = 0,
            BELLMAN = 1
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

        ///<summary>Executes NearestNeighbor-Algorithm on Graph g and at starting node NODE_S</summary>
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
                //Find Neighbor with smallest cost
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
                    tourCosts += edge.GetCapacity();
                    Tour.Add(g.nodes[NODE_S]);
                    return new Tuple<List<Node>, float>(Tour, tourCosts);
                }
            }
            throw new GraphException("NearestNeighbor: Could not find a Tour!");
        }

        ///<summary>Executes DoubleTree-Algorithm on Graph g at starting node NODS_S with MST-Algorithm</summary>
        ///<returns>Tuple with the Tourlist as the first element and the costs as the second element</returns>
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
                //Backtracking
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
                //Depth-Traverse
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
                    //find next unmarked node
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

        ///<summary>Execute BruteForce-Algorithm to find all possible Tours in Graph G from starting node NODE_S</summary>
        ///<param name="MAX_TOURS">Choose the maximum size of Tourlist.</param>
        ///<returns>Returns List of Tours</returns>
        public static List<Tour> BruteForce(Graph g, int NODE_S, float MAX_TOURS)
        {

            Node node = g.nodes[NODE_S];
            float THRESHOLD = float.PositiveInfinity;
            var touren = BruceForceIterative(g, node, MAX_TOURS, ref THRESHOLD);
            return touren;
        }

        ///<summary>Execute BranchAndBound-Algorithm to find possible Tours in Graph G from starting node NODE_S</summary>
        public static List<Tour> BranchAndBound(Graph g, int NODE_S, float MAX_TOURS)
        {
            Node node = g.nodes[NODE_S];
            var result = NearestNeighbor(g, NODE_S);
            float THRESHOLD = result.Item2;
            var touren = BruceForceIterative(g, node, MAX_TOURS, ref THRESHOLD);
            return touren;
        }

        public static List<Tour> BruceForceIterative(Graph g, Node node, float MAX_TOURS, ref float THRESHOLD)
        {
            bool BB = false;
            if (THRESHOLD != float.PositiveInfinity)
                BB = true;

            List<Tour> touren = new List<Tour>();
            int TOURS_FINISHED = 0;
            List<Edge>.Enumerator enumerator = node.edges.GetEnumerator();
            Stack<Tuple<List<Edge>.Enumerator, Tour>> stack = new Stack<Tuple<List<Edge>.Enumerator, Tour>>();
            stack.Push(new Tuple<List<Edge>.Enumerator, Tour>(enumerator, new Tour(node.ID)));


            while (stack.Count != 0)
            {
                var tuple = stack.Pop();
                var edge_loop_enumerator = tuple.Item1;
                var current_tour = tuple.Item2;
                //Close Tour
                if (current_tour.CountStations() == g.NUMBER_OF_NODES())
                {
                    int lastStation = current_tour.GetLastStation();
                    foreach (Edge edge in g.nodes[lastStation].edges)
                    {
                        if (edge.V_TO == node.ID)
                        {
                            current_tour.AddStation(edge.V_TO, edge.GetCapacity());
                            if (current_tour.GetCosts() < THRESHOLD)
                            {
                                if (BB)
                                    THRESHOLD = current_tour.GetCosts();

                                touren.Add(current_tour);
                                TOURS_FINISHED++;
                                if (TOURS_FINISHED >= MAX_TOURS)
                                {
                                    return touren;
                                }
                            }
                            break;
                        }
                    }
                    if (!current_tour.IsFinished())
                    {
                        throw new GraphException("Could not finish the Tour");
                    }
                }
                else if (current_tour.GetCosts() < THRESHOLD)
                {
                    Tuple<List<Edge>.Enumerator, Tour> last = null;
                    //Dive depth into tree 
                    if (edge_loop_enumerator.MoveNext())
                    {
                        if (!current_tour.ContainsStation(edge_loop_enumerator.Current.V_TO))
                        {
                            current_tour.AddStation(edge_loop_enumerator.Current.V_TO, edge_loop_enumerator.Current.GetCapacity());
                            if (current_tour.GetCosts() < THRESHOLD)
                                last = new Tuple<List<Edge>.Enumerator, Tour>(g.nodes[edge_loop_enumerator.Current.V_TO].edges.GetEnumerator(), current_tour);


                        }
                    }
                    //For all alternative tours
                    while (edge_loop_enumerator.MoveNext())
                    {
                        Tour alternativeTour = current_tour.Copy();
                        if (!alternativeTour.ContainsStation(edge_loop_enumerator.Current.V_TO))
                        {
                            alternativeTour.AddStation(edge_loop_enumerator.Current.V_TO, edge_loop_enumerator.Current.GetCapacity());
                            if (current_tour.GetCosts() < THRESHOLD)
                                stack.Push(new Tuple<List<Edge>.Enumerator, Tour>(g.nodes[edge_loop_enumerator.Current.V_TO].edges.GetEnumerator(), alternativeTour));
                        }
                    }
                    if (last != null)
                    {
                        stack.Push(last);
                    }
                }
            }
            return touren;
        }

        ///<summary>Executes Dijkstra Algorithm and returns the shortest path or the shortest path tree</summary>
        ///<param name="g">Graph</param>
        ///<param name="NODE_S">Start node</param>
        ///<param name="NODE_T">Optional paramater for target node. If null, shortest path tree will be created.</param>
        public static GraphUtils.DSPResult DSP(Graph g, int NODE_S, int? NODE_T)
        {

            GraphUtils.DSPResult result = GraphUtils.InitDSP(g, NODE_S);
            //1. stopcondition
            while (result.VQueue.Count > 0)
            {
                int firstNode = result.VQueue.Dequeue();
                Node MIN_NODE = g.nodes[firstNode];
                //2. stopcondition if Node can't be reached
                if (MIN_NODE.DISTANCE == float.PositiveInfinity)
                {
                    return result;
                }
                MIN_NODE.mark();
                //Constructing Tree
                if (MIN_NODE.Predecessor != null)
                {
                    if (g.nodes[MIN_NODE.Predecessor.V_FROM].isMarked())
                    {
                        if (!result.G_neu.nodes.ContainsKey(MIN_NODE.Predecessor.V_FROM))
                            result.G_neu.nodes.Add(MIN_NODE.Predecessor.V_FROM, new Node());

                        result.G_neu.nodes.Add(MIN_NODE.ID, new Node());
                        g.AddEdge(MIN_NODE.Predecessor.V_FROM, MIN_NODE.Predecessor.V_TO, MIN_NODE.Predecessor.GetCapacity());
                    }
                }
                //3. stopcondition if NODE_T found
                if (NODE_T != null)
                    if (MIN_NODE.ID == NODE_T)
                    {
                        result.DISTANCE = g.nodes[NODE_T.GetValueOrDefault()].DISTANCE;
                        return result;
                    }

                //Go through all neighbours and update distances if possible
                foreach (Edge edge in MIN_NODE.edges)
                {
                    if (result.VQueue.Contains(edge.V_TO))
                    {
                        float calculatedDistance = edge.GetCapacity() + MIN_NODE.DISTANCE;
                        if (calculatedDistance < g.nodes[edge.V_TO].DISTANCE)
                        {
                            g.nodes[edge.V_TO].DISTANCE = calculatedDistance;
                            g.nodes[edge.V_TO].Predecessor = edge;
                            result.VQueue.UpdatePriority(edge.V_TO, calculatedDistance);
                        }
                    }
                }
            }
            if (result.VQueue.Count == 0)
                return result;

            throw new GraphException("Could not find shortest path");
        }


        public static GraphUtils.BFSPResult BFSP(Graph g, int NODE_S, int? NODE_T)
        {
            //Step 1
            GraphUtils.BFSPResult result = GraphUtils.InitBFSP(g, NODE_S);
            List<Edge> edges = result.edges;
            int tries = g.NUMBER_OF_NODES() - 1;
            //Step 2
            for (int repeat = 0; repeat < tries; repeat++)
            {
                bool updated = false;

                foreach (Edge edge in edges)
                {
                    if (g.nodes[edge.V_FROM].DISTANCE != float.PositiveInfinity)
                    {
                        float calculatedDistance = g.nodes[edge.V_FROM].DISTANCE + edge.GetCapacity();
                        if (calculatedDistance < g.nodes[edge.V_TO].DISTANCE)
                        {
                            updated = true;
                            g.nodes[edge.V_TO].DISTANCE = calculatedDistance;
                            g.nodes[edge.V_TO].Predecessor = edge;
                        }
                    }
                }
                if (!updated) { break; }
            }
            //Step 3
            foreach (Edge edge in edges)
            {

                float calculatedDistance = g.nodes[edge.V_FROM].DISTANCE + edge.GetCapacity();
                if (calculatedDistance < g.nodes[edge.V_TO].DISTANCE)
                {
                    result.negativeCycleEdge = edge;
                    return result;
                }
                //Constructing Tree
                if (g.nodes[edge.V_TO].Predecessor != null)
                {
                    if (!result.G_neu.nodes.ContainsKey(edge.V_TO))
                        result.G_neu.nodes.Add(edge.V_TO, new Node(edge.V_TO));
                    if (!result.G_neu.nodes.ContainsKey(g.nodes[edge.V_TO].Predecessor.V_FROM))
                        result.G_neu.nodes.Add(g.nodes[edge.V_TO].Predecessor.V_FROM, new Node(g.nodes[edge.V_TO].Predecessor.V_FROM));

                    result.G_neu.AddEdge(g.nodes[edge.V_TO].Predecessor.V_FROM, edge.V_TO, g.nodes[edge.V_TO].Predecessor.GetCapacity());
                }

            }

            if (NODE_T != null)
                result.DISTANCE = g.nodes[NODE_T.GetValueOrDefault()].DISTANCE;

            return result;
        }
    }
}