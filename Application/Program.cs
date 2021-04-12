using MA.Interfaces;
using MA.Classes;
using System.Diagnostics;
namespace MA
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph g = new UndirectedGraph();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            g.ReadFromFile(Config.DATA_DIR + "/Graph1.txt", capacity: false);
            watch.Stop();
            System.Console.WriteLine($"Sekunden vergangen: {watch.ElapsedMilliseconds / 1000}");
            System.Console.WriteLine(g);
        }
    }
}
