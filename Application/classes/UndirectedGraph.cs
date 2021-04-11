
using MA.Interfaces;
using MA.Collections;
using System.Collections.Generic;
using System;
using System.IO;
namespace MA.Classes
{
    public class UndirectedGraph : Graph
    {
        public override void AddEdge(int N1, int N2)
        {
            Node n1 = nodes.GetOrAdd(N1);
            Node n2 = nodes.GetOrAdd(N2);

            n1.AddEdge(N2);
            n2.AddEdge(N1);
            NUMBER_OF_EDGES++;
        }

        public override void AddEdge(int N1, int N2, float capacity)
        {
            throw new NotImplementedException();
        }

        public override void ReadFromFile(string path)
        {
            int LINES_READ = 0;
            const int V_FROM = 0;
            const int V_TO = 1;
            using (StreamReader sr = File.OpenText(path))
            {
                string S_DATA = "";
                //Read first Line to get Number of Entries
                if ((S_DATA = sr.ReadLine()) != null)
                {
                    this.NUMBER_OF_NODES = int.Parse(S_DATA);
                }
                //Read all Nodes from Stream
                while ((S_DATA = sr.ReadLine()) != null)
                {
                    LINES_READ++;
                    string[] VERTICES = S_DATA.Split('\t');
                    this.AddEdge(int.Parse(VERTICES[V_FROM]), int.Parse(VERTICES[V_TO]));
                }
            }
        }

        public override string ToString()
        {
            return $"Undirected Graph\n|V| = {nodes.Count}\n|E| = {NUMBER_OF_EDGES}";
        }
    }
}