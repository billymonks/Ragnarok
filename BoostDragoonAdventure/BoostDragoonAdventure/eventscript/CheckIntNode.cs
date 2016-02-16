using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.eventscript
{
    public class CheckIntNode : EventNode
    {
        public String text;

        public CheckIntNode(String text, EventNode parent)
        {
            type = NodeType.CheckInt;
            this.text = text;
            this.parent = parent;
        }
    }
}
