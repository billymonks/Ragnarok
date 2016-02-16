using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.eventscript
{
    public class SetIntNode : EventNode
    {
        public String key;
        public int val;

        public SetIntNode(String key, int val, EventNode parent)
        {
            type = NodeType.SetInt;
            this.key = key;
            this.val = val;
            this.parent = parent;
        }
    }
}
