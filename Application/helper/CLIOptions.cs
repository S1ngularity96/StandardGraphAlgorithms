using CommandLine;
namespace MA.Helper
{
    public class CLIOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('f', "file", Required = true, HelpText = "Choose file to import Graph")]
        public string File { get; set; }

        [Option('w', "weighted", Required = false, Default = false, HelpText = "Select if edges are weighted")]
        public bool capacity { get; set; }

        [Option('s', "stopwatch", Required = false, Default = false, HelpText = "Use stopwatch and measure time")]
        public bool stopwatch { get; set; }
    }
}