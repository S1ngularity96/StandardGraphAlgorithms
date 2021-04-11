using System.IO;
namespace MA
{
    public static class Config
    {
        public static string SLN_DIR = GetSolutionDirectory();
        public static string DATA_DIR = SLN_DIR + "/data";

        private static string GetSolutionDirectory()
        {
            string current_dir = Directory.GetCurrentDirectory();
            string SLN = Directory.GetParent(current_dir).ToString();
            return SLN;
        }
    }
}