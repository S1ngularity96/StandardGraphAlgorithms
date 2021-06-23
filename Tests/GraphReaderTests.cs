using Xunit;
using Xunit.Abstractions;
using MA.Classes;
using System.IO;
using System.Collections.Generic;
namespace MA.Testing
{
    public class GraphReaderTests
    {
        string SLN_DIR = "/home/andrei/Dokumente/Programmierprojekte/C#/Mathematische_Algorithmen";
        private readonly ITestOutputHelper console;
        public GraphReaderTests(ITestOutputHelper output)
        {
            this.console = output;
        }

        struct NodeT
        {
            public float BALANCE;
            public Node.NodeType type;
            public NodeT(float balance, Node.NodeType type)
            {
                this.BALANCE = balance;
                this.type = type;
            }
        }
        struct EdgeT
        {
            public int V_FROM;
            public int V_TO;
            public float COSTS;
            public float CAP;
            public EdgeT(int from, int to, float costs, float cap)
            {
                this.V_FROM = from;
                this.V_TO = to;
                this.COSTS = costs;
                this.CAP = cap;
            }
        }

        struct GraphT
        {
            public string filepath;
            public List<NodeT> expectedNodes;
            public List<EdgeT> expectedEdges;
        }
        [Fact]
        public void ReadFileWithBalances()
        {

            List<GraphT> graphs = new List<GraphT>(){
                new GraphT {
                     
                    filepath = Path.Join(SLN_DIR, "data", "costminimal", "Kostenminimal2.txt"),
                    expectedNodes = new List<NodeT>(){
                        new NodeT(1.0f, Node.NodeType.SOURCE),
                        new NodeT(-1.0f,Node.NodeType.SINK),
                        new NodeT(0, Node.NodeType.NONE),
                        new NodeT(0, Node.NodeType.NONE),
                        new NodeT(0, Node.NodeType.NONE)
                    },
                    expectedEdges = new List<EdgeT>(){
                        new EdgeT(0,1,2.0f,2.0f),
                        new EdgeT(2,3,1.0f,5.0f),
                        new EdgeT(3,4,-4.0f, 3.0f),
                        new EdgeT(4,2,2.0f, 2.0f)
                    }
                },
                new GraphT {
                    filepath = Path.Join(SLN_DIR, "data", "costminimal", "Kostenminimal1.txt"),
                    expectedNodes = new List<NodeT>(){
                        new NodeT(4.0f, Node.NodeType.SOURCE),
                        new NodeT(-1.0f, Node.NodeType.SINK),
                        new NodeT(0.0f, Node.NodeType.NONE),
                        new NodeT(2.0f, Node.NodeType.SOURCE),
                        new NodeT(-5.0f, Node.NodeType.SINK)
                    },
                    expectedEdges = new List<EdgeT>(){
                        new EdgeT(0, 2, 2.0f, 2.0f),
                        new EdgeT(0, 4, 1.0f, 5.0f),
                        new EdgeT(2,4, -3.0f, 3.0f),
                        new EdgeT(3, 1, 2.0f, 2.0f),
                        new EdgeT(3, 4, 1.0f, 2.0f)
                    }
                }
            };

            foreach(GraphT expG in graphs){
                List<NodeT> expectedNodes = expG.expectedNodes;
                List<EdgeT> expectedEdges = expG.expectedEdges;
                DirectedGraph graph = new DirectedGraph();
                graph.ReadFromBalancedGraph(expG.filepath, false);
                Assert.StrictEqual<int>(expectedNodes.Count, graph.nodes.Count);
                Assert.StrictEqual<int>(expectedEdges.Count, graph.NUMBER_OF_EDGES());

                for (int node = 0; node < expectedEdges.Count; node++)
                {
                    Assert.StrictEqual<float>(expectedNodes[node].BALANCE, graph.nodes[node].GetBalance());
                    Assert.StrictEqual<Node.NodeType>(expectedNodes[node].type, graph.nodes[node].type);
                }
                foreach (EdgeT e in expectedEdges)
                {
                    Edge edge = GraphUtils.GetEdgeFromTo(graph, e.V_FROM, e.V_TO);
                    Assert.StrictEqual<float>(e.COSTS, edge.GetCosts());
                    Assert.StrictEqual<float>(e.CAP, edge.GetCapacity());
                }
            }
        }

       
    }
}