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

            CommandLine.Parser.Default.ParseArguments<CLIOptions>(args).
            WithParsed(RunOptions).
            WithNotParsed(HandleParseError);
        }
        static void RunOptions(CLIOptions options)
        {
            if (options.spdemo)
            {
                ShortestPathDemo();
            }

            if (options.maxflowdemo)
            {
                MaxFlowDemo();
            }

            if (options.mincostdemocc)
            {
                MinCostDemoCC();
            }

            if(options.mincostdemossp){
                MinCostDemoSSP();
            }

            if (options.File != null)
            {
                //... Choose something 
            }
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


        static void ShortestPathDemo()
        {
            string ROOT = System.IO.Path.Join(Config.SLN_DIR, "data");
            Helper.Structs.SPDemoObject[] cases = {
                new Structs.SPDemoObject {
                    filename = System.IO.Path.Join(ROOT,"sp","Wege1.txt"),
                    name = "Wege1",
                    algorithm = Algorithms.SP.DIJKSTRA,
                    direction = Graph.Direction.directed,
                    NODE_S = 2,
                    NODE_T = 0
            },
                new Structs.SPDemoObject{
                    filename = System.IO.Path.Join(ROOT,"sp","Wege2.txt"),
                    name = "Wege2",
                    algorithm = Algorithms.SP.BELLMAN,
                    direction = Graph.Direction.directed,
                    NODE_S = 2,
                    NODE_T = 0
                },
                new Structs.SPDemoObject{
                    filename = System.IO.Path.Join(ROOT, "sp","Wege3.txt"),
                    name = "Wege3",
                    algorithm = Algorithms.SP.BELLMAN,
                    direction = Graph.Direction.directed,
                    NODE_S = 0,
                },
                new Structs.SPDemoObject {
                    filename = System.IO.Path.Join(ROOT,"capacity","G_1_2.txt"),
                    name = "G_1_2",
                    algorithm = Algorithms.SP.BELLMAN,
                    direction = Graph.Direction.directed,
                    NODE_S = 0,
                    NODE_T = 1
                },
                new Structs.SPDemoObject {
                    filename = System.IO.Path.Join(ROOT, "capacity","G_1_2.txt"),
                    name = "G_1_2",
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
                g.ReadFromFile($"{c.filename}", true);
                if (c.algorithm == Algorithms.SP.DIJKSTRA)
                {
                    GraphUtils.DSPResult result = Algorithms.DSP(g, c.NODE_S, c.NODE_T);
                    System.Console.WriteLine($"{c.name} as {c.direction} graph: from node {c.NODE_S} to node {c.NODE_T}: Length {result.DISTANCE} using {c.algorithm}");
                }

                if (c.algorithm == Algorithms.SP.BELLMAN)
                {
                    GraphUtils.BFSPResult result = Algorithms.BFSP(g, c.NODE_S, c.NODE_T, (Edge edge) => { return edge.GetCapacity(); });
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

        static void MaxFlowDemo()
        {

            string ROOT = System.IO.Path.Join(Config.SLN_DIR, "data");
            Helper.Structs.EKDemoObject[] cases = {
                new Structs.EKDemoObject{
                    name = "Fluss",
                    filename = System.IO.Path.Join(ROOT,"flow","Fluss.txt"),
                    NODE_S = 0,
                    NODE_T = 7
                },
                new Structs.EKDemoObject{
                    name = "Fluss2",
                    filename = System.IO.Path.Join(ROOT,"flow","Fluss2.txt"),
                    NODE_S = 0,
                    NODE_T = 7
                },
                new Structs.EKDemoObject{
                    name = "G_1_2",
                    filename = System.IO.Path.Join(ROOT,"capacity","G_1_2.txt"),
                    NODE_S = 0,
                    NODE_T = 7
                }
            };

            foreach (Helper.Structs.EKDemoObject ekcase in cases)
            {
                Graph g = new DirectedGraph();
                g.ReadFromFile(ekcase.filename, true);
                try
                {
                    float maxFlow = Algorithms.EdmondKarp(g, ekcase.NODE_S, ekcase.NODE_T);
                    System.Console.WriteLine($"Graph {ekcase.name}:\t Der maximale Fluß von {ekcase.NODE_S} nach {ekcase.NODE_T} beträgt {maxFlow}.");
                }
                catch (GraphException ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }

        static void MinCostDemoCC()
        {
            DirectedGraph g = new DirectedGraph();
            string ROOT = System.IO.Path.Join(Config.SLN_DIR, "data", "costminimal");
            Helper.Structs.MinCostDemoObject[] cases = {
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal1",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal1.txt")
                },
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal2",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal2.txt")
                },
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal3",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal3.txt")
                },
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal4",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal4.txt")
                },
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal_gross1",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal_gross1.txt")
                },
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal_gross2",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal_gross2.txt")
                },
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal_gross3",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal_gross3.txt")
                }

            };

            foreach (Helper.Structs.MinCostDemoObject mccase in cases)
            {
                try
                {
                    System.Console.ForegroundColor = System.ConsoleColor.Green;
                    g.ReadFromBalancedGraph(mccase.filename, false);
                    float result = Algorithms.CycleCanceling(g);
                    System.Console.WriteLine($"{mccase.name}: {result}");
                }
                catch (BalancedFlowMissingException)
                {
                    System.Console.ForegroundColor = System.ConsoleColor.Red;
                    System.Console.WriteLine($"{mccase.name}: Kein B-Fluss möglich!");
                }
            }
            System.Console.ForegroundColor = System.ConsoleColor.Gray;
        }

        static void MinCostDemoSSP(){
            DirectedGraph g = new DirectedGraph();
            string ROOT = System.IO.Path.Join(Config.SLN_DIR, "data", "costminimal");
            Helper.Structs.MinCostDemoObject[] cases = {
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal1",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal1.txt")
                },
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal2",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal2.txt")
                },
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal3",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal3.txt")
                },
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal4",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal4.txt")
                },
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal_gross1",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal_gross1.txt")
                },
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal_gross2",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal_gross2.txt")
                },
                new Structs.MinCostDemoObject(){
                    name = "Kostenminimal_gross3",
                    filename = System.IO.Path.Join(ROOT, "Kostenminimal_gross3.txt")
                }
            };

            foreach (Helper.Structs.MinCostDemoObject mccase in cases)
            {
                try
                {
                    System.Console.ForegroundColor = System.ConsoleColor.Green;
                    g.ReadFromBalancedGraph(mccase.filename, false);
                    float result = Algorithms.SuccessiveShortestPath(g);
                    System.Console.WriteLine($"{mccase.name}: {result}");
                }
                catch (BalancedFlowMissingException)
                {
                    System.Console.ForegroundColor = System.ConsoleColor.Red;
                    System.Console.WriteLine($"{mccase.name}: Kein B-Fluss möglich!");
                }
            }
            System.Console.ForegroundColor = System.ConsoleColor.Gray;
        }



        static void HandleParseError(IEnumerable<Error> errors)
        {

        }
    }
}
