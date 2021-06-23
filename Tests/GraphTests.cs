using MA;
using MA.Classes;
using MA.Interfaces;
using System.Collections.Generic;
using Xunit;
using System.IO;
using Xunit.Abstractions;

namespace MA.Testing
{
    public class GraphTests
    {
        string SLN_DIR = "/home/andrei/Dokumente/Programmierprojekte/C#/Mathematische_Algorithmen";
        private readonly ITestOutputHelper output;

        public GraphTests(ITestOutputHelper outputHelper)
        {
            this.output = outputHelper;
        }

        [Fact]
        public void Graph1()
        {   
            string filepath = Path.Join(SLN_DIR, "data", "Graph1.txt");
            Graph g = new UndirectedGraph();
            g.ReadFromFile(filepath, false);

            Assert.Equal<int>(15, g.NUMBER_OF_NODES());
            Assert.Equal<int>(17, g.NUMBER_OF_EDGES());
            g.UnmarkAllNodes();
            Assert.Equal<int>(3, GraphUtils.GetUnmarkedNeighbours(g, 0).Count);
            g.nodes[0].mark();
            Assert.Equal<bool>(true, g.nodes[0].isMarked());
            Assert.Equal<int>(1, g.GetFirstUnmarkedNode().ID);
        }


        #region Edge TestCases
        public struct TC_Edges
        {
            public Edge first { get; set; }
            public Edge second { get; set; }
            public bool result { get; set; }
        }
        #endregion

        [Fact]
        public void Edges()
        {

            Edge edge = new Edge(3, 1, 0.0f);
            TC_Edges[] TestCases = {
                new TC_Edges{
                    first = new Edge(4,3,2.3f),
                    second = new Edge(4,3,2.3f),
                    result = true
                },
                new TC_Edges{
                    first = new Edge(3,4,0.4f),
                    second = new Edge(4,3,0.4f),
                    result = true
                },
                new TC_Edges{
                    first = new Edge(3,4,0.0f),
                    second = new Edge(3,4, 1.0f),
                    result = false
                },
                new TC_Edges{
                    first = new Edge(5,3, 0.1f),
                    second = new Edge(3,5, 0.2f),
                    result = false
                }
            };
            //Test if Equals works
            foreach (TC_Edges cases in TestCases)
            {
                Assert.StrictEqual<bool>(cases.result, cases.first.Equals(cases.second));
            }

            List<Edge> priority_queue = new List<Edge>();
            float[] EdgeCapacities = { 4.5f, 1.3f, 44.0f, 0.1f, 7.7f };
            priority_queue.Sort();
            foreach (float capacity in EdgeCapacities)
            {
                priority_queue.Add(new Edge(0, 0, capacity));
            }



        }

        [Fact]
        public void TestResudialGraph()
        {
            int[] nodes = { 0, 1, 2, 3 };
            int S = 0;
            int U = 1;
            int V = 2;
            int T = 3;
            Graph g = new DirectedGraph();
            foreach (var node in nodes) { g.nodes.Add(node, new Node(node)); }

            Edge s_to_u = new Edge(S, U, 2, 2);
            Edge s_to_v = new Edge(S, V, 0, 3);
            Edge s_to_t = new Edge(S, T, 1, 1);
            Edge u_to_v = new Edge(U, V, 1, 2);
            Edge u_to_t = new Edge(U, T, 1, 3);
            Edge v_to_t = new Edge(V, T, 1, 1);
            g.nodes[S].AddEdge(s_to_u);
            g.nodes[S].AddEdge(s_to_v);
            g.nodes[S].AddEdge(s_to_t);
            g.nodes[U].AddEdge(u_to_v);
            g.nodes[U].AddEdge(u_to_t);
            g.nodes[V].AddEdge(v_to_t);
            Graph resudial = FlowAlgorithms.CreateResidualGraph(g);
            var augmented = FlowAlgorithms.BFSPath(resudial, S, T);
            Assert.StrictEqual<int>(3, augmented.pathOfEdges.Count);
            Assert.StrictEqual<float>(1.0f, augmented.minEdge.GetCapacity());
        }
    }
}
