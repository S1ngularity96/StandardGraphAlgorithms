using MA.Exceptions;
using System;
namespace MA.Classes
{
    public class Edge : IComparable
    {
        public int V_FROM { get; set; }
        public int V_TO { get; set; }
        private float CAPACITY = 0.0f;

        public Edge(int V_FROM, int V_TO)
        {
            this.V_FROM = V_FROM;
            this.V_TO = V_TO;
        }

        public Edge(int V_FROM, int V_TO, float capacity)
        {
            this.V_FROM = V_FROM;
            this.V_TO = V_TO;
            this.CAPACITY = capacity;
        }

        public float GetCapacity()
        {
            return this.CAPACITY;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) { throw new GraphException("Can not compare edge with non-existing edge"); }
            if (obj.GetType().Equals(this.GetType()))
            {
                Edge other = (Edge)obj;
                if (((this.V_TO == other.V_TO && this.V_FROM == other.V_FROM) ||
                (this.V_TO == other.V_FROM && this.V_FROM == other.V_TO)) && this.CAPACITY == other.CAPACITY)
                {
                    return true;
                }

            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "{ " + V_FROM + " -> " + V_TO + " }";
        }

        public int CompareTo(object obj)
        {
            if (obj == null) { throw new GraphException("Can not compare edge with non-existing edge"); }
            if (this.CAPACITY >= ((Edge)obj).CAPACITY)
            {
                return 1;
            }
            return -1;
        }
    }
}