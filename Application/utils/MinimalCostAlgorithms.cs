using System;
using System.Collections.Generic;
using System.Text;
using MA.Interfaces;
using MA.Classes;
using MA.Exceptions;
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
        {   string text  = "";
            if(edges == null)
                return "";
            foreach(Edge edge in edges)
            {
                text += $"From: {edge.V_FROM}, To: {edge.V_TO}, Cost: {edge.GetCosts()} Cap: {edge.GetCapacity()}\n";
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
                    Edge forwardEdge = new Edge(edge.V_FROM, edge.V_TO,edge.GetCapacity() - edge.GetFlow(),edge.GetCosts(), forward: true);
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
            foreach(Node node in g.nodes.Values)
            {
                foreach(Edge edge in node.edges)
                {
                    costs += edge.GetCosts() + edge.GetFlow();
                }
            }
            return costs;
        }

        public static GraphUtils.BFSPResult FindNegativeCycle(DirectedGraph g, List<int> sources, List<int> sinks)
        {   
            GraphUtils.BFSPResult result = new GraphUtils.BFSPResult();
            foreach(int source in sources)
            {
                foreach(int sink in sinks)
                {
                    result = Algorithms.BFSP(g, source, sink, (Edge e) => {return e.GetCosts() * e.GetCapacity();});
                    if (result.negativeCycleEdge != null){
                        return result;
                    }
                }
            }
            return result;
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
                supernodeIDs = new int[]{ superSource.ID, superSink.ID }
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
            


            foreach(int sink in sinks)
            {
                graph.nodes[sink].edges = graph.nodes[sink].edges.FindAll((edge) => { return !(edge.V_TO == ssink);});
            }

            return graph;
        }
        public static DirectedGraph RandomB_Flow(DirectedGraph g, List<int> sources, List<int> sinks)
        {
            var supergraph = AddSuperNodes(g, sources, sinks);
            //Edmonds Karp
            supergraph.g.UnmarkAllNodes();
            Algorithms.EdmondKarp(g, supergraph.supernodeIDs[0], supergraph.supernodeIDs[1]);
            //Remove Super-Source/Sink
            var result = RemoveSuperNodes(supergraph, sinks);
            return result;
        }
    }
}
