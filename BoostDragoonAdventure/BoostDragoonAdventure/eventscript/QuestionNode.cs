using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.eventscript
{
    public class QuestionNode : EventNode
    {
        public String text, key;

        public QuestionNode(String text, String key, EventNode parent)
        {
            type = NodeType.Question;
            this.text = text;
            this.key = key;
            this.parent = parent;
        }

        public override void Process(GameBase game, manager.gameplay.GameplayManager gm, player.Player p, EventScript script)
        {
            base.Process(game, gm, p, script);

            script.curr = next;

            List<AnswerNode> answers = new List<AnswerNode>();

            foreach (EventNode node in children)
            {
                if (node is AnswerNode)
                {
                    answers.Add((AnswerNode)node);
                }
            }

            gm.factory.DisplayQuestion(text, key, answers);
        }
    }
}
