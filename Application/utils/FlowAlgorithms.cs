using MA.Classes;
using MA.Interfaces;
using MA.Exceptions;
using System.Collections.Generic;
namespace MA
{
    public static class FlowAlgorithms
    {

        public struct AugmentedPath
        {
            public Edge minEdge;
            public List<Edge> pathOfEdges;
        }

        public static Graph CreateResidualGraph(Graph g)
        {
            Graph G_neu = new DirectedGraph();
            G_neu.nodes = new Collections.NodeSet(g.NUMBER_OF_NODES());
            foreach (Node n in g.nodes.Values)
            {
                foreach (Edge edge in n.edges)
                {
                    Edge forwardEdge = new Edge(edge.V_FROM, edge.V_TO, edge.GetCapacity() - edge.GetFlow(), forward: true);
                    Edge backwardEdge = new Edge(edge.V_TO, edge.V_FROM, edge.GetFlow(), forward: false);
                    if (forwardEdge.GetCapacity() > 0)
                    {
                        G_neu.nodes[forwardEdge.V_FROM].AddEdge(forwardEdge);
                    }

                    if (backwardEdge.GetCapacity() > 0)
                    {
                        G_neu.nodes[backwardEdge.V_FROM].AddEdge(backwardEdge);
                    }
                }
            }
            return G_neu;
        }

        public static Graph UpdateFlows(AugmentedPath p, Graph g)
        {

            float yMin = p.minEdge.GetCapacity();
            foreach (Edge edge in p.pathOfEdges)
            {
                if (edge.isResidualBackward())
                {
                    foreach (Edge orig_edge in g.nodes[edge.V_TO].edges)
                    {
                        if (edge.V_TO == orig_edge.V_FROM && edge.V_FROM == orig_edge.V_TO)
                        {
                            orig_edge.SetFlow(orig_edge.GetCapacity() - yMin);
                        }
                    }
                }
                else
                {
                    foreach (Edge orig_edge in g.nodes[edge.V_FROM].edges)
                    {
                        if (edge.V_FROM == orig_edge.V_FROM && edge.V_TO == edge.V_TO)
                        {
                            orig_edge.SetFlow(orig_edge.GetCapacity() + yMin);
                        }
                    }
                }
            }
            return g;
        }




        public static AugmentedPath BFSPath(Graph g, int S, int T)
        {

            if (S == T)
            {
                throw new GraphException("Startnode can't be the targetnode");
            }

            g.UnmarkAllNodes();
            AugmentedPath augpath = new AugmentedPath();
            //Dictionary<ParentNodeID, PathEdges>
            Dictionary<int, List<Edge>> paths = new Dictionary<int, List<Edge>>();
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(g.nodes[S]);
            while (queue.Count != 0)
            {
                Node node = queue.Dequeue();
                node.mark();
                foreach (Edge edge in node.edges)
                {
                    if (!g.nodes[edge.V_TO].isMarked())
                    {
                        g.nodes[edge.V_TO].mark();
                        if (paths.ContainsKey(node.ID))
                        {
                            paths[node.ID].Add(edge);
                        }
                        else
                        {
                            paths.Add(node.ID, new List<Edge>() { edge });
                        }

                        if (g.nodes[edge.V_TO].ID == T)
                        {
                            //finish search
                            augpath.pathOfEdges = paths[node.ID];
                            augpath.minEdge = paths[node.ID][0];
                            foreach (Edge e in paths[node.ID])
                            {
                                if (e.GetCapacity() < augpath.minEdge.GetCapacity())
                                {
                                    augpath.minEdge = e;
                                    
                                }
                            }
                            return augpath;
                        }

                        queue.Enqueue(g.nodes[edge.V_TO]);
                    }
                }
            }
            augpath.pathOfEdges = null;
            return augpath;
        }
    }
}