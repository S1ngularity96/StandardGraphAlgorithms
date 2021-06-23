
using System;
namespace MA.Exceptions{
    public class BalancedFlowMissingException : Exception
    {
        public BalancedFlowMissingException(string msg) : base(msg) { }
    }
}