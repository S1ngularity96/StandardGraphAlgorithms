using MA.Exceptions;
namespace MA.Classes
{
    public class Edge
    {
        bool WITH_CAPACITY = false;
        private int NODE_ID;
        private float CAPACITY = 0.0f;

        public Edge(int NODE_ID)
        {
            this.NODE_ID = NODE_ID;
        }

        public Edge(int nodeId, float capacity)
        {
            this.NODE_ID = nodeId;
            this.CAPACITY = capacity;
            this.WITH_CAPACITY = true;
        }

        public int GetPointedNodeID()
        {
            return NODE_ID;
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