using MA;
using MA.Classes;
using MA.Interfaces;
using System.Collections.Generic;
using Xunit;

namespace MA.Testing
{
    public class GraphTests
    {
        [Fact]
        public void Graph1()
        {
            Graph g = new UndirectedGraph();
            g.ReadFromFile("C:/Users/Livem/Documents/Programmierprojekte/CSharp/GraphAlgorithms/data/Graph1.txt", false);

            Assert.Equal<int>(15, g.NUMBER_OF_NODES());
            Assert.Equal<int>(17, g.NUMBER_OF_EDGES);
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
    }
}
