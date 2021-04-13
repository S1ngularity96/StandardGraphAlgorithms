using MA;
using MA.Classes;
using MA.Interfaces;
using System;
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
            Assert.Equal<int>(3, g.nodes[0].GetUnmarkedNeigbours().Count);

            g.nodes[0].mark();
            Assert.Equal<bool>(true, g.nodes[0].isMarked());
            Assert.Equal<int>(1, g.GetFirstUnmarkedNode().ID);
        }
    }
}
