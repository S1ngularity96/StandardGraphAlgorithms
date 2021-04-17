using System.Collections.Generic;
namespace MA.Collections
{
    public class DisjointSetCollection
    {
        Dictionary<int, int> setIDs;
        Dictionary<int, List<int>> sets;



        public DisjointSetCollection(int NUMBER_OF_NODES)
        {
            setIDs = new Dictionary<int, int>();
            sets = new Dictionary<int, List<int>>();
            for (int node = 0; node < NUMBER_OF_NODES; node++)
            {
                setIDs.Add(node, node);
                sets.Add(node, new List<int>() { node });
            }
        }

        #region Operations
        public List<int> findSet(int node)
        {
            if (this.sets.ContainsKey(node))
            {
                return this.sets[node];
            }
            return null;
        }

        public void removeSet(int setID)
        {
            sets[setID].Clear();
            sets.Remove(setID);
        }

        public bool union(int node_a, int node_b)
        {
            int node_a_setID = setIDs[node_a];
            int node_b_setID = setIDs[node_b];

            if (!(node_a_setID == node_b_setID))
            {
                List<int> nodes_a = findSet(node_a_setID);
                List<int> nodes_b = findSet(node_b_setID);

                if (nodes_a != null || nodes_b != null)
                {
                    if (node_a_setID < node_b_setID)
                    {
                        foreach (int node in nodes_b)
                        {
                            nodes_a.Add(node);
                            setIDs[node] = node_a_setID;
                        }
                        removeSet(node_b_setID);
                        return true;
                    }
                    else
                    {
                        foreach (int node in nodes_a)
                        {
                            nodes_b.Add(node);
                            setIDs[node] = node_b_setID;
                        }
                        removeSet(node_a_setID);
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        public int NUMBER_OF_SETS()
        {
            return this.sets.Count;
        }
    }
}