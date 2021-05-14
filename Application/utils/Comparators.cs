using MA.Classes;
namespace MA
{
    public static class Comparators
    {
        public static int NodeDistanceComparator(Node x, Node y)
        {

            if (x.DISTANCE == y.DISTANCE)
            {
                return 0;
            }
            if (x.DISTANCE <= y.DISTANCE)
            {
                return 1;
            }
            return -1;
        }
    }
}