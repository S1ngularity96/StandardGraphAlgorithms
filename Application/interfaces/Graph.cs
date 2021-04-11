using MA.Collections;
namespace MA.Interfaces
{
    public abstract class Graph
    {
        public NodeSet nodes = new NodeSet();
        public string GraphPath { get; set; }
        public int NUMBER_OF_NODES { get; set; }
        public int NUMBER_OF_EDGES { get; set; }
        public abstract void ReadFromFile(string path);
        public abstract void AddEdge(int N1, int N2);
        public abstract void AddEdge(int N1, int N2, float capacity);
    }
}