using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.eventscript
{
    public class IntValNode : EventNode
    {
        public int val;

        public IntValNode(int val, EventNode parent)
        {
            type = NodeType.IntVal;
            this.val = val;
            this.parent = parent;
        }
    }
}
