using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.eventscript
{
    public class AnswerNode : EventNode
    {
        public String text;
        public int val;

        public AnswerNode(String text, int val, EventNode parent)
        {
            type = NodeType.Answer;
            this.text = text;
            this.val = val;
            this.parent = parent;
        }
    }
}
