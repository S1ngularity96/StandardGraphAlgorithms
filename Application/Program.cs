using MA.Interfaces;
using MA.Classes;
using MA.Helper;
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
            ENABLE_TIME_MEASUREMENTS = options.stopwatch;
            Graph g = new UndirectedGraph();
            Diagnostic.MeasureTime(() => { g.ReadFromFile(options.File, options.capacity); });
            System.Console.WriteLine(g);
            Diagnostic.MeasureTime(() =>
            {
                var components = Algorithms.BreadthSearch(g);
                System.Console.WriteLine($"Graph consists of {components} components.");
            });
        }

        static void HandleParseError(IEnumerable<Error> errors)
        {

        }
    }
}
