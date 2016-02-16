using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.eventscript
{
    public class QuestionNode : EventNode
    {
        public String text;

        public QuestionNode(String text, EventNode parent)
        {
            type = NodeType.Question;
            this.text = text;
            this.parent = parent;
        }
    }
}
