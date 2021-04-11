using System.Collections.Generic;
using MA.Classes;
namespace MA.Collections
{
    public class NodeSet : Dictionary<int, Node>
    {
        public void Push(int key, Node value)
        {
            if (!this.ContainsKey(key))
            {
                this.Add(key, value);
            }
        }

        public Node GetOrAdd(int key)
        {
            if (this.ContainsKey(key))
            {
                return this[key];
            }
            Node node = new Node();
            this.Add(key, node);
            return node;
        }
    }
}