using MA.Interfaces;
namespace MA.Helper
{
    public static class Structs
    {
        public struct SPDemoObject
        {
            public string name;
            public Algorithms.SP algorithm;
            public Graph.Direction direction;
            public int NODE_S;
            public int? NODE_T;
        }

        public struct EKDemoObject {
            public string name;
            public string filename;
            public int NODE_S;
            public int NODE_T;

        }
    }

}