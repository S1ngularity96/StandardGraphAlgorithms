using MA.Collections;
using System.IO;
namespace MA.Interfaces
{
    public abstract class Graph
    {
        public NodeSet nodes = null;
        public string GraphPath { get; set; }
        public int NUMBER_OF_NODES()
        {
            if (this.nodes == null) { return 0; };
            return this.nodes.Count;
        }
        public int NUMBER_OF_EDGES { get; set; }
        public void ReadFromFile(string path, bool capacity)
        {
            int LINES_READ = 0;
            const int V_FROM = 0;
            const int V_TO = 1;
            const int CAP_INDEX = 2;
            using (StreamReader sr = File.OpenText(path))
            {
                string S_DATA = "";
                //Read first Line to get Number of Entries
                if ((S_DATA = sr.ReadLine()) != null)
                {
                    int NUMBER_OF_NODES = int.Parse(S_DATA);
                    this.nodes = new NodeSet(NUMBER_OF_NODES);
                }
                //Read all Nodes from Stream
                while ((S_DATA = sr.ReadLine()) != null)
                {
                    LINES_READ++;
                    string[] VERTICES = S_DATA.Split('\t');
                    if (capacity)
                    {
                        this.AddEdge(int.Parse(VERTICES[V_FROM]), int.Parse(VERTICES[V_TO]), float.Parse(VERTICES[CAP_INDEX]));
                    }
                    else
                    {
                        this.AddEdge(int.Parse(VERTICES[V_FROM]), int.Parse(VERTICES[V_TO]), 0.0f);
                    }
                }
            }
        }
        public abstract void AddEdge(int N1, int N2, float capacity);
    }
}