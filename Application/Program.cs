using MA.Interfaces;
using MA.Classes;
using MA.Helper;
using MA.Exceptions;
using CommandLine;
using System.Collections.Generic;

namespace MA
{
    class Program
    {

        public static bool ENABLE_TIME_MEASUREMENTS = false;
        static void Main(string[] args)
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            MaxFlowDemo();
            // CommandLine.Parser.Default.ParseArguments<CLIOptions>(args).
            // WithParsed(RunOptions).
            // WithNotParsed(HandleParseError);
        }
        static void RunOptions(CLIOptions options)
        {
            ENABLE_TIME_MEASUREMENTS = options.stopwatch;
            

        }

        static void BruteForce(CLIOptions options, bool BB = false)
        {
            Graph g = new UndirectedGraph();
            g.ReadFromFile(options.File, options.capacity);
            System.Console.WriteLine(g);
            Diagnostic.MeasureTime(() =>
            {
                List<Collections.Tour> touren = null;
                float min = float.PositiveInfinity;
                int t_index = 0;
                if (BB)
                {
                    touren = Algorithms.BranchAndBound(g, 0, MAX_TOURS: float.PositiveInfinity);
                    System.Console.WriteLine(touren[touren.Count - 1]);
                }
                else
                {
                    touren = Algorithms.BruteForce(g, 0, MAX_TOURS: float.PositiveInfinity);
                    System.Console.WriteLine($"{touren.Count} tours found");
                    for (int i_tuple = 0; i_tuple < touren.Count; i_tuple++)
                    {
                        var costs = touren[i_tuple].GetCosts();
                        if (costs < min)
                        {
                            t_index = i_tuple;
                            min = costs;
                        }
                    }
                    System.Console.WriteLine(touren[t_index]);
                }
            });
        }

        static void ShortestPathDemo(CLIOptions options)
        {
            string ROOT = "C:\\Users\\Livem\\Documents\\Programmierprojekte\\CSharp\\GraphAlgorithms\\data";
            Helper.Structs.SPDemoObject[] cases = {
                new Structs.SPDemoObject {
                    name = "sp\\Wege1.txt",
                    algorithm = Algorithms.SP.DIJKSTRA,
                    direction = Graph.Direction.directed,
                    NODE_S = 2,
                    NODE_T = 0
            },
                new Structs.SPDemoObject{
                    name = "sp\\Wege2.txt",
                    algorithm = Algorithms.SP.BELLMAN,
                    direction = Graph.Direction.directed,
                    NODE_S = 2,
                    NODE_T = 0
                },
                new Structs.SPDemoObject{
                    name = "sp\\Wege3.txt",
                    algorithm = Algorithms.SP.BELLMAN,
                    direction = Graph.Direction.directed,
                    NODE_S = 0,
                },
                new Structs.SPDemoObject {
                    name = "capacity\\G_1_2.txt",
                    algorithm = Algorithms.SP.BELLMAN,
                    direction = Graph.Direction.directed,
                    NODE_S = 0,
                    NODE_T = 1
                },
                new Structs.SPDemoObject {
                    name = "capacity\\G_1_2.txt",
                    algorithm = Algorithms.SP.BELLMAN,
                    direction = Graph.Direction.undirected,
                    NODE_S = 0,
                    NODE_T = 1
                }
            };

            Graph g = null;
            foreach (var c in cases)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Gray;
                if (c.direction == Graph.Direction.directed) { g = new DirectedGraph(); }
                else { g = new UndirectedGraph(); }
                g.ReadFromFile($"{ROOT}\\{c.name}", true);
                if (c.algorithm == Algorithms.SP.DIJKSTRA)
                {
                    GraphUtils.DSPResult result = Algorithms.DSP(g, c.NODE_S, c.NODE_T);
                    System.Console.WriteLine($"{c.name} as {c.direction} graph: from node {c.NODE_S} to node {c.NODE_T}: Length {result.DISTANCE} using {c.algorithm}");
                }

                if (c.algorithm == Algorithms.SP.BELLMAN)
                {
                    GraphUtils.BFSPResult result = Algorithms.BFSP(g, c.NODE_S, c.NODE_T);
                    if (result.negativeCycleEdge != null)
                    {
                        System.Console.ForegroundColor = System.ConsoleColor.Red;
                        System.Console.WriteLine($"Negative cycle exists in {c.name} and was found in {result.negativeCycleEdge}");
                    }
                    else
                    {
                        System.Console.WriteLine($"{c.name} as {c.direction} graph: from node {c.NODE_S} to node {c.NODE_T}: Length {result.DISTANCE} using {c.algorithm}");
                    }

                }
            }
        }

        static void MaxFlowDemo(){
            
            string ROOT = "/home/andrei/Dokumente/Programmierprojekte/C#/Mathematische_Algorithmen/data";
            Helper.Structs.EKDemoObject[] cases = {
                new Structs.EKDemoObject{
                    name = "Fluss",
                    filename = ROOT+"/flow/Fluss.txt",
                    NODE_S = 0,
                    NODE_T = 7
                },
                new Structs.EKDemoObject{
                    name = "Fluss2",
                    filename = ROOT+"/flow/Fluss2.txt",
                    NODE_S = 0,
                    NODE_T = 7
                },
                new Structs.EKDemoObject{
                    name = "G_1_2",
                    filename = ROOT+"/capacity/G_1_2.txt",
                    NODE_S = 0,
                    NODE_T = 7
                }
            };

            foreach(Helper.Structs.EKDemoObject ekcase in cases){
                Graph g = new DirectedGraph();
                g.ReadFromFile(ekcase.filename, true);
                try{
                    float maxFlow = Algorithms.EdmondKarp(g, ekcase.NODE_S, ekcase.NODE_T);
                    System.Console.WriteLine($"Graph {ekcase.name}:\t Der maximale Fluß von {ekcase.NODE_S} nach {ekcase.NODE_T} beträgt {maxFlow}.");

                }catch(GraphException ex){
                    System.Console.WriteLine(ex.Message);
                }
            }
        }

        static void HandleParseError(IEnumerable<Error> errors)
        {

        }
    }
}
