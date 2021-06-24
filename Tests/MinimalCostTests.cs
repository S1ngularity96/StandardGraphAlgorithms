using Xunit;
using MA.Classes;
using MA.Collections;
using System.Collections.Generic;
using System.IO;
using System;
namespace MA.Testing
{
    public class MinimalCostTests
    {
        string TESTS_DIR = "C:/Users/Livem/Documents/Programmierprojekte/CSharp/GraphAlgorithms/Tests";
        string SLN_DIR = "C:/Users/Livem/Documents/Programmierprojekte/CSharp/GraphAlgorithms";

        struct EdgeT
        {
            public float flow;
            public float cap;
            public float cost;
            public int V_TO;
            public EdgeT(int V_TO, float flow, float cap, float cost)
            {
                this.V_TO = V_TO;
                this.flow = flow;
                this.cap = cap;
                this.cost = cost;
            }
        }
        struct NodeT
        {
            public int ID;
            public float balance;
            public Node.NodeType type;
            public List<EdgeT> Edges;
        }

        struct ResudialEdgeT
        {
            public int V_FROM;
            public int V_TO;
            public bool forward;
            public float cost;
            public float capacity;

            public ResudialEdgeT(int V_FROM, int V_TO, float capacity, float cost, bool forward)
            {
                this.V_FROM = V_FROM;
                this.V_TO = V_TO;
                this.forward = forward;
                this.cost = cost;
                this.capacity = capacity;
            }
        }

        struct GraphT
        {
            public List<NodeT> nodes;
        }

        public DirectedGraph CreateGraphWithoutBFlow()
        {
            GraphT graphT = new GraphT()
            {
                nodes = new List<NodeT>(){
                    new NodeT(){ID = 0, balance = 8.0f, // A
                    Edges = new List<EdgeT>(){
                        new EdgeT(4, 0,6,5),
                        new EdgeT(2, 0,5,2)
                    }},
                    new NodeT(){ID = 1, balance = 7.0f, //B
                    Edges = new List<EdgeT>(){
                        new EdgeT(2,0,5,5),
                        new EdgeT(3,0,5,3)
                    }},
                    new NodeT(){ID = 2, balance = 0.0f, // C
                    Edges = new List<EdgeT>(){
                        new EdgeT(4,0,4,2),
                        new EdgeT(5,0,5,2),
                        new EdgeT(3,0,5,-1)
                    }},
                    new NodeT(){ID = 3, balance = 0.0f, // D
                    Edges = new List<EdgeT>(){
                        new EdgeT(5, 0,7,2)
                    }},
                    new NodeT(){ID = 4, balance = -6.0f }, // E
                    new NodeT(){ID = 5, balance = -9.0f }  // F
                }
            };


            DirectedGraph g = new DirectedGraph();
            foreach (NodeT nodeT in graphT.nodes)
            {
                var node = new Node(nodeT.ID);
                node.SetBalance(nodeT.balance);
                if (nodeT.Edges != null)
                {
                    foreach (EdgeT edge in nodeT.Edges)
                    {
                        node.AddEdge(new Edge(nodeT.ID, edge.V_TO, edge.flow, edge.cap, edge.cost));
                    }
                }

                g.nodes.Push(node.ID, node);
            }
            return g;
        }

        public DirectedGraph CreateGraphOne()
        {
            GraphT graphT = new GraphT()
            {
                nodes = new List<NodeT>(){
                    new NodeT(){ID = 0, balance = 8.0f, // A
                    Edges = new List<EdgeT>(){
                        new EdgeT(4, 3,6,5),
                        new EdgeT(2, 5,5,2)
                    }},
                    new NodeT(){ID = 1, balance = 7.0f, //B
                    Edges = new List<EdgeT>(){
                        new EdgeT(2,3,5,5),
                        new EdgeT(3,4,5,3)
                    }},
                    new NodeT(){ID = 2, balance = 0.0f, // C
                    Edges = new List<EdgeT>(){
                        new EdgeT(4,3,4,2),
                        new EdgeT(5,5,5,2),
                        new EdgeT(3,0,5,-1)
                    }},
                    new NodeT(){ID = 3, balance = 0.0f, // D
                    Edges = new List<EdgeT>(){
                        new EdgeT(5, 4,7,2)
                    }},
                    new NodeT(){ID = 4, balance = -6.0f }, // E
                    new NodeT(){ID = 5, balance = -9.0f }  // F
                }
            };


            DirectedGraph g = new DirectedGraph();
            foreach (NodeT nodeT in graphT.nodes)
            {
                var node = new Node(nodeT.ID);
                node.SetBalance(nodeT.balance);
                if (nodeT.Edges != null)
                {
                    foreach (EdgeT edge in nodeT.Edges)
                    {
                        node.AddEdge(new Edge(nodeT.ID, edge.V_TO, edge.flow, edge.cap, edge.cost));
                    }
                }

                g.nodes.Push(node.ID, node);
            }
            return g;
        }

        public DirectedGraph ResudialGraph(DirectedGraph g)
        {
            return MinimalCostAlgorithms.CreateResidualGraph(g);
        }

        [Fact]
        public void CreateResudialGraph()
        {
            var g = CreateGraphOne();
            var G_neu = ResudialGraph(g);
            List<ResudialEdgeT> redges = new List<ResudialEdgeT>(){
                new ResudialEdgeT(0,4,3,5,true), //A-E
                new ResudialEdgeT(1,2,2,5,true), //B-C
                new ResudialEdgeT(1,3,1,3,true), //B-D
                new ResudialEdgeT(2, 4, 1,2, true), //C-E
                new ResudialEdgeT(2,3, 5, -1, true), //C-D
                new ResudialEdgeT(2,0, 5, -2, false), //C-A
                new ResudialEdgeT(2,1,3,-5,false), //C-B
                new ResudialEdgeT(3,1,4,-3, false), //D-B
                new ResudialEdgeT(3,5,3,2, true), //D-F
                new ResudialEdgeT(4, 0,3, -5, false), //E-A            
                new ResudialEdgeT(4,2,3,-2, false), //E-C
                new ResudialEdgeT(5,3,4,-2, false), //F-D
                new ResudialEdgeT(5,2,5,-2, false), //F-C
            };

            foreach (ResudialEdgeT re in redges)
            {
                Edge edge = GraphUtils.GetEdgeFromTo(G_neu, re.V_FROM, re.V_TO);
                Assert.StrictEqual<float>(re.cost, edge.GetCosts());
                Assert.StrictEqual<float>(re.capacity, edge.GetCapacity());
                Assert.StrictEqual<bool>(re.forward, edge.isResidualForward());

            }
        }

        [Fact]
        public void TestSuperNodes()
        {
            var g = CreateGraphWithoutBFlow();
            var g_copy = CreateGraphWithoutBFlow();
            var sources = GraphUtils.GetNodeIdsOfType(g, Node.NodeType.SOURCE);
            var sinks = GraphUtils.GetNodeIdsOfType(g, Node.NodeType.SINK);

            Assert.Contains<int>(0, sources);
            Assert.Contains<int>(1, sources);
            Assert.Contains<int>(4, sinks);
            Assert.Contains<int>(5, sinks);

            var supergraph = MinimalCostAlgorithms.AddSuperNodes(g, sources, sinks);
            var g_old = MinimalCostAlgorithms.RemoveSuperNodes(supergraph, sinks);

            int n_total = g.NUMBER_OF_NODES();
            for (int node = 0; node < n_total; node++)
            {
                Assert.StrictEqual<int>(g_copy.nodes[node].ID, g.nodes[node].ID);
                int e_total = g_copy.nodes[node].edges.Count;
                Assert.StrictEqual<int>(e_total, g.nodes[node].edges.Count);
                for (int edge = 0; edge < e_total; edge++)
                {
                    Edge e_copy = g_copy.nodes[node].edges[edge];
                    Edge e_new = g.nodes[node].edges[edge];

                    Assert.True(e_copy.Equals(e_new), "Edges are not equal ");
                }
            }
        }

        [Fact]
        public void RandomBFlow()
        {
            var g = CreateGraphWithoutBFlow();
            var sources = GraphUtils.GetNodeIdsOfType(g, Node.NodeType.SOURCE);
            var sinks = GraphUtils.GetNodeIdsOfType(g, Node.NodeType.SINK);

            Assert.Contains<int>(0, sources);
            Assert.Contains<int>(1, sources);
            Assert.Contains<int>(4, sinks);
            Assert.Contains<int>(5, sinks);

            var b_flow = MinimalCostAlgorithms.RandomB_Flow(g, sources, sinks);

        }

        [Fact]
        public void FindNegativeCycle()
        {
            var g = CreateGraphOne();
            var resudial = ResudialGraph(g);

            var result = MinimalCostAlgorithms.FindNegativeCycle(resudial);
            Assert.True(result.found);

            File.WriteAllText(Path.Join(TESTS_DIR, "negative_path.log"), MinimalCostAlgorithms.PrintNegativeCycleEdges(result.path));
            File.AppendAllText(Path.Join(TESTS_DIR, "negative_path.log"), $"y=min ={result.y_min}");

        }

        [Fact]
        public void CycleCanceling()
        {

            File.WriteAllText(Path.Join(TESTS_DIR, "cc.log"), "CycleCanceling:\n");
            DirectedGraph g = new DirectedGraph();
            g.ReadFromBalancedGraph(Path.Join(SLN_DIR, "data", "costminimal", "Kostenminimal1.txt"), false);
            List<int> sources = GraphUtils.GetNodeIdsOfType(g, Node.NodeType.SOURCE);
            List<int> sinks = GraphUtils.GetNodeIdsOfType(g, Node.NodeType.SINK);
            float costs = 0.0f;
            try
            {
                int tries = 1000;
                int currentTry = 1;
                DirectedGraph g_bflow = MinimalCostAlgorithms.RandomB_Flow(g, sources, sinks);
                File.AppendAllText(Path.Join(TESTS_DIR, "cc.log"), "b_flow:\n"+g_bflow.GraphToString());
                DirectedGraph g_residual = MinimalCostAlgorithms.CreateResidualGraph(g_bflow);
                File.AppendAllText(Path.Join(TESTS_DIR, "cc.log"), "\n\nresidual:\n" + g_residual.GraphToString());
                GraphUtils.NegativeCycleResult cycle = MinimalCostAlgorithms.FindNegativeCycle(g_residual);
                File.AppendAllText(Path.Join(TESTS_DIR, "cc.log"), "\n\nnegative cycle:\n" + MinimalCostAlgorithms.PrintNegativeCycleEdges(cycle.path));
                while (cycle.found && (currentTry != tries))
                {
                    g_bflow = MinimalCostAlgorithms.UpdateFlows(g_bflow, cycle);
                    File.AppendAllText(Path.Join(TESTS_DIR, "cc.log"), "b_flow:\n" + g_bflow.GraphToString());
                    g_residual = MinimalCostAlgorithms.CreateResidualGraph(g_bflow);
                    File.AppendAllText(Path.Join(TESTS_DIR, "cc.log"), "\n\nresidual:\n" + g_residual.GraphToString());
                    cycle = MinimalCostAlgorithms.FindNegativeCycle(g_residual);
                    File.AppendAllText(Path.Join(TESTS_DIR, "cc.log"), "\n\nnegative cycle:\n" + MinimalCostAlgorithms.PrintNegativeCycleEdges(cycle.path));
                    currentTry++;
                }
                costs = MinimalCostAlgorithms.CalculateFlowCosts(g_bflow);
            }
            catch (Exception ex)
            {

            }

            Assert.StrictEqual<float>(3.0f, costs);

        }
    }
}