using MA.Interfaces;
using MA.Classes;
using MA.Helper;
using CommandLine;
using System;
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
            ENABLE_TIME_MEASUREMENTS = options.stopwatch;
            ShortestPath(options);

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

        static void ShortestPath(CLIOptions options)
        {
            Graph g = new DirectedGraph();
            g.ReadFromFile(options.File, options.capacity);
            GraphUtils.SPValues sP = new GraphUtils.SPValues();
            Diagnostic.MeasureTime(() =>
            {
                sP = Algorithms.BFSP(g, 0, 1);
                if (sP.negativeCycleEdge != null)
                {
                    System.Console.WriteLine("es gibt einen negativen Zykel");
                }
                else
                {
                    System.Console.WriteLine($"Wege2 als gerichteter Graph: von Knoten 0 zu Knoten 1: Länge {sP.DISTANCE}");
                    System.Console.WriteLine(sP.G_neu.NUMBER_OF_NODES());
                }

            });


        }

        static void HandleParseError(IEnumerable<Error> errors)
        {

        }
    }
}
