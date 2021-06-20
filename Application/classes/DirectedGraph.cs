using MA.Interfaces;
using MA.Exceptions;
using MA.Collections;
using System.IO;
using System.Globalization;

namespace MA.Classes
{
    public class DirectedGraph : Graph
    {

        public override void AddEdge(int N1, int N2, float capacity)
        {
            if (this.nodes == null)
            {
                throw new GraphException("No nodes in Graph exist");
            }
            Node n1 = nodes.GetOrAdd(N1);
            n1.AddEdge(N1, N2, capacity);

        }

        public override int NUMBER_OF_EDGES()
        {
            int countedEdges = 0;
            if (this.nodes != null)
            {
                foreach (Node n in this.nodes.Values)
                {
                    countedEdges += n.edges.Count;
                }
            }

            return countedEdges;
        }

        public void ReadFromBalancedGraph(string path, bool log = false)
        {
            if (!File.Exists((path)))
            {
                throw new FileNotFoundException($"{path} does not exist");
            }
            if (log)
                System.Console.WriteLine($"Importing Graph from file {Path.GetFileName(path)} ...");

            const int V_FROM = 0;
            const int V_TO = 1;
            const int COSTS = 2;
            const int CAP_INDEX = 3;
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
                int CURRENT_NODE = 0;
                while ((S_DATA = sr.ReadLine()) != null)
                {
                    float balance = float.Parse(S_DATA, CultureInfo.InvariantCulture.NumberFormat);
                    this.nodes[CURRENT_NODE].SetBalance(balance);
                    CURRENT_NODE++;
                    if (CURRENT_NODE == NUMBER_OF_NODES()) { break; }
                }

                while ((S_DATA = sr.ReadLine()) != null)
                {
                    string[] ELEMENT = S_DATA.Split('\t');
                    if (ELEMENT.Length == 4)
                    {
                        Node node = this.nodes[int.Parse(ELEMENT[V_FROM])];
                        node.AddEdge(new Edge(
                            int.Parse(ELEMENT[V_FROM]),
                            int.Parse(ELEMENT[V_TO]),
                            float.Parse(ELEMENT[COSTS], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(ELEMENT[CAP_INDEX], CultureInfo.InvariantCulture.NumberFormat),
                            0.0f));
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"Directed Graph\n|V| = {nodes.Count}\n|E| = {NUMBER_OF_EDGES()}";
        }


    }
}