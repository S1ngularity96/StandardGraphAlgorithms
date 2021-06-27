using Xunit;
using MA.Classes;
using MA.Exceptions;
using System.Collections.Generic;
using System.IO;
namespace MA.Testing
{
    public class MinimalCostTests
    {


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
            public float r_balance;
            public float b_diff;
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

        struct MinCostTestObject
        {
            public string name;
            public string filename;
            public float expectedValue;
            public bool expectedException;
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

        private DirectedGraph CreateFromGraphT(GraphT graphT)
        {
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


            return CreateFromGraphT(graphT);
        }


        public DirectedGraph CreateGraphTwo()
        {

            DirectedGraph graph = new DirectedGraph();
            GraphT graphT = new GraphT()
            {
                nodes = new List<NodeT>(){
                    new NodeT(){ID = 0, balance = 6.0f, // A
                        Edges = new List<EdgeT>(){
                            new EdgeT(1, 0,4,2),
                            new EdgeT(2, 0,3,4)
                        }
                    },
                    new NodeT(){ID = 1, balance = 0.0f, //B
                        Edges = new List<EdgeT>(){
                            new EdgeT(3,0,2,1),
                            new EdgeT(2,0,3,-1.0f)
                        }
                    },
                    new NodeT(){ID = 2, balance = -2.0f, //C
                        Edges = new List<EdgeT>(){
                            new EdgeT(3, 0,4,3)
                        }
                    },
                    new NodeT(){ID = 3, balance = -4.0f //D

                    }
                }
            };
            return CreateFromGraphT(graphT);
            
        }

        public DirectedGraph ResudialGraph(DirectedGraph g)
        {
            return MinimalCostAlgorithms.CreateResidualGraphCC(g);
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

            File.WriteAllText(Path.Join(Config.TESTS_DIR, "negative_path.log"), MinimalCostAlgorithms.PrintNegativeCycleEdges(result.path));
            File.AppendAllText(Path.Join(Config.TESTS_DIR, "negative_path.log"), $"y=min ={result.y_min}");

        }

        [Fact]
        public void CycleCanceling()
        {

            DirectedGraph g = new DirectedGraph();
            string ROOT = System.IO.Path.Join(Config.SLN_DIR, "data", "costminimal");
            MinCostTestObject[] cases = {
                new MinCostTestObject(){
                    name = "Kostenminimal1",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal1.txt"),
                    expectedException = false,
                    expectedValue = 3
                },
                new MinCostTestObject(){
                    name = "Kostenminimal2",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal2.txt"),
                    expectedException = false,
                    expectedValue = 0
                },
                new MinCostTestObject(){
                    name = "Kostenminimal3",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal3.txt"),
                    expectedException = true
                },
                new MinCostTestObject(){
                    name = "Kostenminimal4",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal4.txt"),
                    expectedException = true
                },
                new MinCostTestObject(){
                    name = "Kostenminimal_gross1",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal_gross1.txt"),
                    expectedValue = 1537
                },
                new MinCostTestObject(){
                    name = "Kostenminimal_gross2",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal_gross2.txt"),
                    expectedValue = 1838
                },
                new MinCostTestObject(){
                    name = "Kostenminimal_gross3",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal_gross3.txt"),
                    expectedException = true
                }

            };

            foreach (MinCostTestObject mccase in cases)
            {

                System.Console.ForegroundColor = System.ConsoleColor.Green;
                g.ReadFromBalancedGraph(mccase.filename, false);

                if (mccase.expectedException)
                {
                    Assert.Throws<BalancedFlowMissingException>(() =>
                    {
                        Algorithms.CycleCanceling(g);
                    });
                }
                else
                {
                    float result = Algorithms.CycleCanceling(g);
                    Assert.StrictEqual<float>(mccase.expectedValue, result);
                }


            }
        }

        [Fact]
        public void CreateResudialGraphSSP()
        {
            var g = CreateGraphTwo();
            var g_ssp = MinimalCostAlgorithms.InitSSP(g);
            var residual = MinimalCostAlgorithms.CreateResidualGraphSSP(g_ssp);

            List<ResudialEdgeT> redges = new List<ResudialEdgeT>(){
                new ResudialEdgeT(0,1,4,2,true), //A-B
                new ResudialEdgeT(0,2,3,4,true), //A-C
                new ResudialEdgeT(2,1, 3,1,false), //C-B
                new ResudialEdgeT(2,3,4,3, true), //C-D,
                new ResudialEdgeT(1,3,2,1, true) //B-D
            };

            List<NodeT> nodes = new List<NodeT>(){
                new NodeT(){ID= 0, balance = 6, r_balance = 0, b_diff=6.0f},
                new NodeT(){ID= 1, balance=0, r_balance= 3, b_diff= -3.0f},
                new NodeT(){ID= 2, balance=-2.0f, r_balance=-3.0f, b_diff=1.0f},
                new NodeT(){ID= 3, balance=-4.0f, r_balance=0.0f, b_diff=-4.0f}
            };

            foreach (ResudialEdgeT re in redges)
            {
                Edge edge = GraphUtils.GetEdgeFromTo(residual, re.V_FROM, re.V_TO);
                Assert.StrictEqual<float>(re.cost, edge.GetCosts());
                Assert.StrictEqual<float>(re.capacity, edge.GetCapacity());
                Assert.StrictEqual<bool>(re.forward, edge.isResidualForward());

            }

            foreach(NodeT node in nodes){
                Node r_node = residual.nodes[node.ID];
                Assert.StrictEqual<float>(node.balance, r_node.GetBalance());
                Assert.StrictEqual<float>(node.r_balance, r_node.GetR_Balance());
                Assert.StrictEqual<float>(node.b_diff, r_node.GetBalance() - r_node.GetR_Balance());
            }
        }

        public void SuccessiveShortestPath()
        {
            DirectedGraph g = new DirectedGraph();
            string ROOT = System.IO.Path.Join(Config.SLN_DIR, "data", "costminimal");
            MinCostTestObject[] cases = {
                new MinCostTestObject(){
                    name = "Kostenminimal1",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal1.txt"),
                    expectedException = false,
                    expectedValue = 3
                },
                new MinCostTestObject(){
                    name = "Kostenminimal2",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal2.txt"),
                    expectedException = false,
                    expectedValue = 0
                },
                new MinCostTestObject(){
                    name = "Kostenminimal3",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal3.txt"),
                    expectedException = true
                },
                new MinCostTestObject(){
                    name = "Kostenminimal4",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal4.txt"),
                    expectedException = true
                },
                new MinCostTestObject(){
                    name = "Kostenminimal_gross1",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal_gross1.txt"),
                    expectedValue = 1537
                },
                new MinCostTestObject(){
                    name = "Kostenminimal_gross2",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal_gross2.txt"),
                    expectedValue = 1838
                },
                new MinCostTestObject(){
                    name = "Kostenminimal_gross3",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal_gross3.txt"),
                    expectedException = true
                }

            };

            foreach (MinCostTestObject mccase in cases)
            {

                System.Console.ForegroundColor = System.ConsoleColor.Green;
                g.ReadFromBalancedGraph(mccase.filename, false);
                File.WriteAllText(Path.Join(Config.TESTS_DIR, "ssp.log"), $"{mccase.name}\n{mccase.filename}\n\n");

                if (mccase.expectedException)
                {
                    Assert.Throws<BalancedFlowMissingException>(() => { Algorithms.SuccessiveShortestPath(g); });

                }
                else
                {
                    try
                    {
                        float result = Algorithms.SuccessiveShortestPath(g);
                        Assert.StrictEqual<float>(mccase.expectedValue, result);
                    }
                    catch (BalancedFlowMissingException ex)
                    {
                        Assert.False(true, $"Exception should not be thrown here!!!\n{ex.Message}");
                    }
                    catch (GraphException ex)
                    {
                        Assert.False(true, $"Exception should not be thrown here!!!\n{ex.Message}");
                    }

                }


            }
        }

    }
}
