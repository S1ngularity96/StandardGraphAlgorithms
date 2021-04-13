using MA.Interfaces;
using MA.Classes;
using System.Diagnostics;
namespace MA
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = "/Graph_ganzganzgross.txt";
            Graph g = new UndirectedGraph();
            g.ReadFromFile(Config.DATA_DIR + filename, capacity: false);
            System.Console.WriteLine(g);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var components = Algorithms.DepthSearch(g);
            watch.Stop();
            System.Console.WriteLine($"Graph {filename} consists of {components} components. Sek:{watch.ElapsedMilliseconds / 1000}");
        }
    }
}
