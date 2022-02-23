using System;
using System.Collections.Generic;
using System.Text;
using MA.Interfaces;
using MA.Classes;
using MA.Exceptions;
using System.Linq;
namespace MA
{
    public static class MinimalCostAlgorithms
    {

        public struct SuperNodesGraph
        {
            public DirectedGraph g;
            public int supersource;
            public int supersink;
        }

        public struct SSPMinResult
        {
            public float y_min;
            public List<Edge> path;
            public bool pathExists;
        }

        public struct SSPPair
        {
            public int source;
            public int target;
            public bool found;
        }

        public static string PrintNegativeCycleEdges(List<Edge> edges)
        {
            string text = "";
            if (edges == null)
                return "";
            foreach (Edge edge in edges)
            {
                text += $"From: {edge.V_FROM}, To: {edge.V_TO}, Cost: {edge.GetCosts()} Cap: {edge.GetCapacity()} Forwar: {edge.isResidualForward()}\n";
            }
            return text;
        }

        public static float CalculateFlowCosts(DirectedGraph g)
        {
            float costs = 0.0f;
            foreach (Node node in g.nodes.Values)
            {
                foreach (Edge edge in node.edges)
                {
                    costs += edge.GetCosts() * edge.GetFlow();
                }
            }
            return costs;
        }

        public static List<int> FindNodesWithNegativeEdges(DirectedGraph g)
        {
            List<int> nodes = new List<int>();
            foreach (Node node in g.nodes.Values)
            {
                foreach (Edge edge in node.edges)
                {
                    if (edge.GetCosts() < 0)
                    {
                        nodes.Add(edge.V_FROM);
                        break;
                    }
                }
            }
            return nodes;
        }

        public static SuperNodesGraph AddSuperNodes(DirectedGraph g, List<int> sources, List<int> sinks)
        {

            Node superSource = g.AddNode();
            Node superSink = g.AddNode();

            foreach (int source in sources)
            {
                Node observedSource = g.nodes[source];
                superSource.AddEdge(new Edge(superSource.ID, observedSource.ID, 0.0f, observedSource.GetBalance()));
            }
            foreach (int sink in sinks)
            {
                Node observedSink = g.nodes[sink];
                observedSink.AddEdge(new Edge(observedSink.ID, superSink.ID, 0.0f, Math.Abs(observedSink.GetBalance())));
            }
            return new SuperNodesGraph()
            {
                g = g,
                supersink = superSink.ID,
                supersource = superSource.ID
            };

        }
        public static DirectedGraph RemoveSuperNodes(SuperNodesGraph supergraph, List<int> sinks)
        {
            int ssource = supergraph.supersource;
            int ssink = supergraph.supersink;
            DirectedGraph graph = supergraph.g;

            graph.nodes.Remove(ssource);
            graph.nodes.Remove(ssink);

            foreach (int sink in sinks)
            {
                graph.nodes[sink].edges = graph.nodes[sink].edges.FindAll((edge) => { return !(edge.V_TO == ssink); });
            }

            return graph;
        }


        #region CycleCanceling
        public static DirectedGraph CreateResidualGraphCC(DirectedGraph g)
        {
            DirectedGraph G_neu = new DirectedGraph();
            G_neu.nodes = new Collections.NodeSet(g.NUMBER_OF_NODES());
            foreach (Node n in g.nodes.Values)
            {
                foreach (Edge edge in n.edges)
                {
                    if (edge.GetFlow() == edge.GetCapacity())
                    {
                        Edge backwardEdge = new Edge(edge.V_TO, edge.V_FROM, edge.GetCapacity(), edge.GetCosts() * -1, forward: false);
                        G_neu.nodes[backwardEdge.V_FROM].AddEdge(backwardEdge);

                    }
                    else if (edge.GetFlow() == 0)
                    {
                        Edge forwardEdge = new Edge(edge.V_FROM, edge.V_TO, edge.GetCapacity(), edge.GetCosts(), forward: true);
                        G_neu.nodes[forwardEdge.V_FROM].AddEdge(forwardEdge);

                    }
                    else if (edge.GetFlow() < edge.GetCapacity())
                    {
                        Edge forwardEdge = new Edge(edge.V_FROM, edge.V_TO, edge.GetCapacity() - edge.GetFlow(), edge.GetCosts(), forward: true);
                        Edge backwardEdge = new Edge(edge.V_TO, edge.V_FROM, edge.GetFlow(), edge.GetCosts() * -1, forward: false);
                        G_neu.nodes[forwardEdge.V_FROM].AddEdge(forwardEdge);
                        G_neu.nodes[backwardEdge.V_FROM].AddEdge(backwardEdge);

                    }
                }
            }
            return G_neu;
        }

        public static DirectedGraph UpdateFlowsCC(DirectedGraph graph, GraphUtils.NegativeCycleResult cycle)
        {
            float y_min = cycle.y_min;
            List<Edge> edges = cycle.path;



            foreach (Edge edge in edges)
            {
                if (edge.isResidualForward())
                {
                    Edge e = GraphUtils.GetEdgeFromTo(graph, edge.V_FROM, edge.V_TO);
                    e.SetFlow(e.GetFlow() + y_min);
                }
                else if (edge.isResidualBackward())
                {
                    Edge e = GraphUtils.GetEdgeFromTo(graph, edge.V_TO, edge.V_FROM);
                    e.SetFlow(e.GetFlow() - y_min);
                }
            }
            return graph;
        }

        public static GraphUtils.NegativeCycleResult FindNegativeCycle(DirectedGraph g)
        {
            GraphUtils.NegativeCycleResult cycleResult = new GraphUtils.NegativeCycleResult();
            List<int> nodes = FindNodesWithNegativeEdges(g);

            foreach (int source in nodes)
            {
                GraphUtils.BFSPResult result = Algorithms.BFSP(g, source, null, (Edge e) => { return e.GetCosts(); });

                if (result.negativeCycleEdge != null)
                {
                    Graph g_res = result.G_neu;
                    Edge e_res = result.negativeCycleEdge;
                    Node pre = g.nodes[e_res.V_TO];
                    HashSet<int> set = new HashSet<int>();
                    int StartTargetNode = -1;

                    //Backtrack path to find loop
                    for (int node = 0; node < g_res.NUMBER_OF_NODES(); node++)
                    {
                        if (set.Add(pre.ID))
                        {
                            pre = g.nodes[pre.Predecessor.V_FROM];
                        }
                        else
                        {
                            StartTargetNode = pre.ID;
                            break;
                        }
                    }

                    //Go through loop to find y_min and edges
                    if (StartTargetNode != -1)
                    {
                        //init values
                        cycleResult.y_min = float.PositiveInfinity;
                        cycleResult.path  = new List<Edge>();

                        Node currentNode = g.nodes[StartTargetNode];
                        Edge predecessor = currentNode.Predecessor;
                        cycleResult.path.Add(predecessor);
                        cycleResult.y_min = predecessor.GetCapacity();
                        currentNode = g.nodes[predecessor.V_FROM];

                        while (currentNode.ID != StartTargetNode)
                        {
                            predecessor = currentNode.Predecessor;
                            cycleResult.path.Add(predecessor);
                            currentNode = g.nodes[predecessor.V_FROM];
                            float p_cap = predecessor.GetCapacity();
                            if (p_cap < cycleResult.y_min) { cycleResult.y_min = p_cap; }
                        }
                        cycleResult.found = true;
                        return cycleResult;
                    }
                }
            }
            return cycleResult;
        }

        public static bool HasBalancedFlow(SuperNodesGraph supergraph, List<int> sources, List<int> sinks)
        {
            DirectedGraph g = supergraph.g;
            foreach (int source in sources)
            {
                Edge e = GraphUtils.GetEdgeFromTo(g, supergraph.supersource, source);
                if (e.GetCapacity() != e.GetFlow())
                    return false;
            }

            foreach (int sink in sinks)
            {
                Edge e = GraphUtils.GetEdgeFromTo(g, sink, supergraph.supersink);
                if (e.GetCapacity() != e.GetFlow())
                    return false;
            }

            return true;
        }

        public static DirectedGraph RandomB_Flow(DirectedGraph g, List<int> sources, List<int> sinks)
        {
            var supergraph = AddSuperNodes(g, sources, sinks);
            supergraph.g.UnmarkAllNodes();
            Algorithms.EdmondKarp(supergraph.g, supergraph.supersource, supergraph.supersink);
            if (!HasBalancedFlow(supergraph, sources, sinks))
                throw new BalancedFlowMissingException("Balanced flow can not be created");

            var result = RemoveSuperNodes(supergraph, sinks);
            return result;
        }

        #endregion


        #region SuccessiveShortestPath
        public static SSPMinResult FindShortestPath(DirectedGraph g, int source, int sink)
        {
            SSPMinResult minResult = new SSPMinResult();
            minResult.path = new List<Edge>();
            minResult.pathExists = false;

            GraphUtils.BFSPResult result = Algorithms.BFSP(g, source, sink, (Edge e) => { return e.GetCosts(); });
            if (result.G_neu.nodes[sink].Predecessor != null)
            {
                minResult.pathExists = true;
                Node pre = result.G_neu.nodes[sink];
                minResult.path.Insert(0, pre.Predecessor);
                while (pre.ID != source)
                {
                    pre = result.G_neu.nodes[pre.Predecessor.V_FROM];
                    if (pre.Predecessor != null)
                        minResult.path.Insert(0, pre.Predecessor);
                }

                float bS = g.nodes[source].GetBalance() - g.nodes[source].GetR_Balance();
                float bT = Math.Abs(g.nodes[sink].GetBalance() - g.nodes[sink].GetR_Balance());
                minResult.y_min = bS < bT ? bS : bT;

                foreach (Edge edge in minResult.path)
                {
                    if (edge.GetCapacity() < minResult.y_min)
                    {
                        minResult.y_min = edge.GetCapacity();
                    }
                }
            }


            return minResult;
        }

        public static DirectedGraph UpdateFlowsSSP(DirectedGraph g, SSPMinResult sSPMinResult)
        {
            float y_min = sSPMinResult.y_min;
            foreach (Edge edge in sSPMinResult.path)
            {
                if (edge.isResidualForward())
                {
                    Edge e = GraphUtils.GetEdgeFromTo(g, edge.V_FROM, edge.V_TO);
                    e.SetFlow(e.GetFlow() + y_min);
                }
                else if (edge.isResidualBackward())
                {
                    Edge e = GraphUtils.GetEdgeFromTo(g, edge.V_TO, edge.V_FROM);
                    e.SetFlow(e.GetFlow() - y_min);
                }
            }
            return g;
        }

        public static DirectedGraph InitSSP(DirectedGraph g)
        {
            foreach (Node node in g.nodes.Values)
            {
                foreach (Edge edge in node.edges)
                {
                    if (edge.GetCosts() >= 0)
                    {
                        edge.SetFlow(0.0f);
                    }
                    else if (edge.GetCosts() < 0)
                    {
                        edge.SetFlow(edge.GetCapacity());
                    }
                }
            }
            return g;
        }

        public static DirectedGraph CreateResidualGraphSSP(DirectedGraph g, bool init)
        {
            DirectedGraph residual = CreateResidualGraphCC(g);
            if (init)
            {
                foreach (Node node in g.nodes.Values)
                {
                    residual.nodes[node.ID].SetBalance(node.GetBalance());
                    foreach (Edge edge in node.edges)
                    {
                        if (init)
                        {
                            if (edge.GetCosts() < 0)
                            {
                                Node r_vfrom = residual.nodes[edge.V_FROM];
                                Node r_vto = residual.nodes[edge.V_TO];
                                r_vfrom.SetR_Balance(r_vfrom.GetR_Balance() - edge.GetCapacity());
                                r_vto.SetR_Balance(r_vto.GetR_Balance() + edge.GetCapacity());
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Node node in residual.nodes.Values)
                {
                    node.SetBalance(g.nodes[node.ID].GetBalance());
                    foreach (Edge edge in node.edges)
                    {
                        if (edge.isResidualBackward())
                        {
                            Node R_VFROM = residual.nodes[edge.V_FROM];
                            Node R_VTO = residual.nodes[edge.V_TO];
                            R_VFROM.SetR_Balance(R_VFROM.GetR_Balance() - edge.GetCapacity());
                            R_VTO.SetR_Balance(R_VTO.GetR_Balance() + edge.GetCapacity());
                        }

                    }
                }
            }
            return residual;
        }

        public static SSPPair FindSSPPair(DirectedGraph g, DirectedGraph residualgraph)
        {
            SSPPair pair = new SSPPair();
            //TODO: Find Pair
            List<int> sources = new List<int>();
            List<int> targets = new List<int>();
            foreach (Node node in residualgraph.nodes.Values)
            {

                if ((node.GetBalance() - node.GetR_Balance()) > 0)
                {
                    sources.Add(node.ID);
                }

                else if ((node.GetBalance() - node.GetR_Balance()) < 0)
                {
                    targets.Add(node.ID);
                }
            }

            foreach (int source in sources)
            {
                foreach (int target in targets)
                {
                    if (Algorithms.IsReachable(residualgraph, source, target))
                    {
                        pair.source = source;
                        pair.target = target;
                        pair.found = true;
                        return pair;
                    }

                }
            }

            return pair;
        }
        #endregion




    }
}
