using System;
namespace MA.Exceptions
{
    public class GraphException : Exception
    {
        public GraphException(string msg) : base(msg) { }
    }
}