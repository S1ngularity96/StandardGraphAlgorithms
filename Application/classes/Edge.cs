using MA.Exceptions;
using MA.Classes;
namespace MA.Classes
{
    public class Edge
    {
        bool WITH_CAPACITY = false;
        private Node node;
        private float CAPACITY = 0.0f;

        public Edge(Node node)
        {
            this.node = node;
        }

        public Edge(Node node, float capacity)
        {
            this.node = node;
            this.CAPACITY = capacity;
            this.WITH_CAPACITY = true;
        }

        public Node GetPointedNode()
        {
            return this.node;
        }

        public float GetCapacity()
        {
            if (WITH_CAPACITY)
            {
                return this.CAPACITY;
            }
            throw new GraphException("Capacity can't be returned. Edge is not weighted!");
        }
    }
}