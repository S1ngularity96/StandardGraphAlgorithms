using MA.Collections;
using MA.Classes;
using System.Collections.Generic;
using System.IO;
namespace MA.Interfaces
{
    public abstract class Graph
    {
        public NodeSet nodes = null;
        public int NUMBER_OF_NODES()
        {
            if (this.nodes == null) { return 0; };
            return this.nodes.Count;
        }
        public int NUMBER_OF_EDGES { get; set; }

        public void UnmarkAllNodes()
        {
            foreach (KeyValuePair<int, Node> pair in nodes)
            {
                pair.Value.unmark();
            }
        }
        public Node GetFirstUnmarkedNode()
        {
            foreach (KeyValuePair<int, Node> pair in nodes)
            {
                if (!pair.Value.isMarked())
                {
                    return pair.Value;
                }
            }
            return null;
        }
        public void ReadFromFile(string path, bool capacity)
        {
            if (!File.Exists((path)))
            {
                return;
            }
            System.Console.WriteLine($"Importing Graph from file {Path.GetFileName(path)} ...");
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
        public abstract void AddEdge(int n1, int n2, float capacity);

    }
}