using System.Collections.Generic;
using MA.Classes;
namespace MA.Collections
{
    public class PriorityQueue : List<Edge>
    {
        enum _ORDER
        {
            MIN = 1,
            MAX = 2
        }
        bool ALLOW_DUPLICATES = true;
        _ORDER order = _ORDER.MIN;

        public PriorityQueue()
        {
            this.Sort();
        }



    }
}