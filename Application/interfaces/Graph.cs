using MA.Collections;
using MA.Classes;
using System.Collections.Generic;
using System;
using System.IO;
using System.Globalization;
namespace MA.Interfaces
{
    public abstract class Graph
    {
        [Flags]
        public enum Direction
        {
            directed = 0,
            undirected = 1
        }
        public NodeSet nodes = null;
        public int NUMBER_OF_NODES()
        {
            if (this.nodes == null) { return 0; };
            return this.nodes.Count;
        }

        public abstract int NUMBER_OF_EDGES();


        public Graph()
        {
            this.nodes = new NodeSet();
        }

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
        public void ReadFromFile(string path, bool capacity, bool log = false)
        {
            if (!File.Exists((path)))
            {
                throw new FileNotFoundException($"{path} does not exist");
            }
            if (log)
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
                        this.AddEdge(int.Parse(VERTICES[V_FROM]), int.Parse(VERTICES[V_TO]), float.Parse(VERTICES[CAP_INDEX], CultureInfo.InvariantCulture.NumberFormat));
                    }
                    else
                    {
                        this.AddEdge(int.Parse(VERTICES[V_FROM]), int.Parse(VERTICES[V_TO]), 0.0f);
                    }

                }
            }
        }
        public abstract void AddEdge(int n1, int n2, float capacity);

        public string NodesToString(){
            string text = "";
            foreach(Node node in nodes.Values){
                text += $"ID: {node.ID}\t\t Edges: {node.edges.Count} \t\t Marked: {node.isMarked()} \t\t Type: {node.type}\n";
            }
            return text;
        }

        public string EdgesToString()
        {
            string text = "";
            foreach (Node node in nodes.Values)
            {
                foreach (Edge edge in node.edges)
                {
                    text += $"V_FROM: {edge.V_FROM}\t\t V_TO:{edge.V_TO}\t\t CAP:{edge.GetCapacity()}\t\t FLOW: {edge.GetFlow()}\t\t Cost: {edge.GetCosts()} \t\tFORWARD: {edge.isResidualForwad()}\n";
                }
            }
            return text;
        }

        public string GraphToString()
        {
            string text = "";
            foreach (Node node in nodes.Values)
            {
                text += $"\nID: {node.ID}\t\t Edges: {node.edges.Count} \tMarked: {node.isMarked()} \t\t Type: {node.type}\n";
                foreach (Edge edge in node.edges)
                {
                    text += $"V_TO:{edge.V_TO}\t\t CAP:{edge.GetCapacity()}\t\t FLOW: {edge.GetFlow()}\t\t Cost: {edge.GetCosts()} \t\tFORWARD: {edge.isResidualForwad()}\n";
                }
            }
            return text;
        }

    }
}