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

        public struct Parent
        {
            public Edge edge;
            public int node;
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

        public static Graph UpdateFlows(AugmentedPath p, Graph g)
        {

            float yMin = p.minEdge.GetCapacity();
            List<Edge> pathOfEdges = new List<Edge>(p.pathOfEdges);

            foreach (Edge edge in pathOfEdges)
            {
                if (edge.isResidualBackward())
                {
                    foreach (Edge orig_edge in g.nodes[edge.V_TO].edges)
                    {
                        if (orig_edge.V_FROM == edge.V_TO && orig_edge.V_TO == edge.V_FROM)
                        {

                            orig_edge.SetFlow(orig_edge.GetFlow() - yMin);
                        }
                    }


                }
                else if (edge.isResidualForwad())
                {
                    foreach (Edge orig_edge in g.nodes[edge.V_FROM].edges)
                    {
                        if (orig_edge.V_FROM == edge.V_FROM && orig_edge.V_TO == edge.V_TO)
                        {
                            orig_edge.SetFlow(orig_edge.GetFlow() + yMin);
                        }
                    }
                }
            }
            return g;
        }


        public static List<Edge> GetPath(Dictionary<int, Parent> paths, int S, int T)
        {
            List<Edge> path = new List<Edge>();
            Parent p = paths[T];


            while (p.node != S)
            {
                path.Insert(0, p.edge);
                p = paths[p.node];
            }

            if (p.node == S)
            {
                path.Insert(0, p.edge);
                return path;
            }

            throw new GraphException("Could not get shortest path in Edmond Karp");
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
            Dictionary<int, Parent> parents = new Dictionary<int, Parent>();
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
                        parents[edge.V_TO] = new Parent { edge = edge, node = edge.V_FROM };
                        if (g.nodes[edge.V_TO].ID == T)
                        {
                            //finish search
                            augpath.pathOfEdges = GetPath(parents, S, T);
                            augpath.minEdge = new Edge(0, 0, float.PositiveInfinity);
                            foreach (Parent parent in parents.Values)
                            {
                                if (parent.edge.GetCapacity() < augpath.minEdge.GetCapacity())
                                {
                                    augpath.minEdge = parent.edge;
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