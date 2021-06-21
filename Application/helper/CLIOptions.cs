using CommandLine;
namespace MA.Helper
{
    public class CLIOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('f', "file", Required = false, HelpText = "Choose file to import Graph")]
        public string File { get; set; }

        [Option('w', "weighted", Required = false, Default = false, HelpText = "Select if edges are weighted")]
        public bool capacity { get; set; }

        [Option('s', "stopwatch", Required = false, Default = false, HelpText = "Use stopwatch and measure time")]
        public bool stopwatch { get; set; }

        [Option("spdemo", Required = false, Default = false, HelpText = "Runs shortest path demo")]
        public bool spdemo { get; set; }
        [Option("flowdemo", Required = false, Default = false, HelpText = "Runs max flow demo with edmond-karp Algorithm")]
        public bool maxflowdemo { get; set; }

        [Option("mincost", Required = false, Default = false, HelpText = "Runs min-cost calculation demo")]
        public bool mincostdemo { get; set; }
    }
}