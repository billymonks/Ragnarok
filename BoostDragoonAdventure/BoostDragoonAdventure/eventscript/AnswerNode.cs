using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.eventscript
{
    public class AnswerNode : EventNode
    {
        public String text;

        public AnswerNode(String text, EventNode parent)
        {
            type = NodeType.Answer;
            this.text = text;
            this.parent = parent;
        }
    }
}
