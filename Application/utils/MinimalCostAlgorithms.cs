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
            public int[] supernodeIDs;
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

        public static DirectedGraph CreateResidualGraph(DirectedGraph g)
        {
            DirectedGraph G_neu = new DirectedGraph();
            G_neu.nodes = new Collections.NodeSet(g.NUMBER_OF_NODES());
            foreach (Node n in g.nodes.Values)
            {
                foreach (Edge edge in n.edges)
                {
                    Edge forwardEdge = new Edge(edge.V_FROM, edge.V_TO, edge.GetCapacity() - edge.GetFlow(), edge.GetCosts(), forward: true);
                    Edge backwardEdge = new Edge(edge.V_TO, edge.V_FROM, edge.GetFlow(), edge.GetCosts() * -1, forward: false);

                    if (forwardEdge.GetCapacity() != 0)
                    {
                        G_neu.nodes[forwardEdge.V_FROM].AddEdge(forwardEdge);
                    }

                    if (backwardEdge.GetCapacity() != 0)
                    {
                        G_neu.nodes[backwardEdge.V_FROM].AddEdge(backwardEdge);
                    }
                }
            }
            return G_neu;
        }


        public static float CalculateFlowCosts(DirectedGraph g)
        {
            float costs = 0.0f;
            foreach (Node node in g.nodes.Values)
            {
                foreach (Edge edge in node.edges)
                {
                    costs += edge.GetCosts() + edge.GetFlow();
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

        public static DirectedGraph UpdateFlows(DirectedGraph graph, GraphUtils.NegativeCycleResult cycle){
            float y_min = cycle.y_min;
            List<Edge> edges = cycle.path;

            foreach(Edge edge in edges){
                if(edge.isResidualForward()){
                    Edge e = GraphUtils.GetEdgeFromTo(graph, edge.V_FROM, edge.V_TO);
                    e.SetFlow(e.GetFlow() + y_min);
                }else if(edge.isResidualBackward()){
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
                    HashSet<int> set = new HashSet<int>();
                    int StartTargetNode = -1;


                    //Backtrack path to find loop
                    for (int node = 0; node < g_res.NUMBER_OF_NODES(); node++)
                    {
                        if (set.Add(e_res.V_TO))
                        {
                            e_res = g.nodes[e_res.V_FROM].Predecessor;
                        }
                        else
                        {
                            StartTargetNode = e_res.V_TO;
                            break;
                        }
                    }

                    //Go through loop to find y_min and edges
                    if (StartTargetNode != -1)
                    {
                        float y_min = float.PositiveInfinity;
                        List<Edge> cycle = new List<Edge>();
                        Node currentNode = g.nodes[StartTargetNode];
                        Edge predecessor = currentNode.Predecessor;
                        cycle.Add(predecessor);
                        y_min = predecessor.GetCapacity();
                        currentNode = g.nodes[predecessor.V_FROM];

                        while (currentNode.ID != StartTargetNode)
                        {
                            predecessor = currentNode.Predecessor;
                            cycle.Add(predecessor);
                            currentNode = g.nodes[predecessor.V_FROM];
                            float p_cap = predecessor.GetCapacity();
                            if (p_cap < y_min) { y_min = p_cap; }
                        }
                        cycleResult.found = true;
                        cycleResult.path = cycle;
                        cycleResult.y_min = y_min;
                        return cycleResult;
                    }
                }
            }

            return cycleResult;
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
                supernodeIDs = new int[] { superSource.ID, superSink.ID }
            };

        }
        public static DirectedGraph RemoveSuperNodes(SuperNodesGraph supergraph, List<int> sinks)
        {
            int ssource = supergraph.supernodeIDs[0];
            int ssink = supergraph.supernodeIDs[1];
            DirectedGraph graph = supergraph.g;
            int[] supernodes = supergraph.supernodeIDs;

            graph.nodes.Remove(ssource);
            graph.nodes.Remove(ssink);



            foreach (int sink in sinks)
            {
                graph.nodes[sink].edges = graph.nodes[sink].edges.FindAll((edge) => { return !(edge.V_TO == ssink); });
            }

            return graph;
        }
        public static DirectedGraph RandomB_Flow(DirectedGraph g, List<int> sources, List<int> sinks)
        {
            var supergraph = AddSuperNodes(g, sources, sinks);
            //Edmonds Karp
            supergraph.g.UnmarkAllNodes();
            Algorithms.EdmondKarp(supergraph.g, supergraph.supernodeIDs[0], supergraph.supernodeIDs[1]);
            //Remove Super-Source/Sink
            var result = RemoveSuperNodes(supergraph, sinks);
            return result;
        }
    }
}
