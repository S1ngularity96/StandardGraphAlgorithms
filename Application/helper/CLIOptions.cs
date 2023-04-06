using CommandLine;
using MA.Types;
namespace MA.Helper
{
    public class CLIOptions
    {
        public ALGORITHM selectedAlgorithm = ALGORITHM.NONE;


        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('f', "file", Required = false, HelpText = "Choose file to import Graph")]
        public string File { get; set; }

        [Option('w', "weighted", Required = false, Default = false, HelpText = "Select if edges are weighted")]
        public bool capacity { get; set; }

        [Option('s', "stopwatch", Required = false, Default = false, HelpText = "Use stopwatch and measure time")]
        public bool stopwatch { get; set; }

        [Option("spdemo", Required = false, HelpText = "Runs shortest path demo")]
        public bool spdemo { get { return ALGORITHM.SHORTEST_PATH == selectedAlgorithm; } set { selectedAlgorithm = ALGORITHM.SHORTEST_PATH; } }
        [Option("flowdemo", Required = false, HelpText = "Runs max flow demo with edmond-karp Algorithm")]
        public bool maxflowdemo { get { return ALGORITHM.FLOW == selectedAlgorithm; } set { selectedAlgorithm = ALGORITHM.FLOW; } }

        [Option("mincostcc", Required = false, HelpText = "Runs cycle-canceling calculation demo")]
        public bool mincostdemocc { get { return ALGORITHM.MC_CYCLE_CANCELING == selectedAlgorithm; } set { selectedAlgorithm = ALGORITHM.MC_CYCLE_CANCELING; } }

        [Option("mincostssp", Required = false, HelpText = "Runs successive-shortes-path calculation demo")]
        public bool mincostdemossp { get { return ALGORITHM.MC_SUCCESSIVE_SHORTEST_PATH == selectedAlgorithm; } set { selectedAlgorithm = ALGORITHM.MC_SUCCESSIVE_SHORTEST_PATH; } }
    }
}