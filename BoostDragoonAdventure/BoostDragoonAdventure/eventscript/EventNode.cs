using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.gameplay;
using wickedcrush.player;

namespace wickedcrush.eventscript
{
    public enum NodeType
    {
        Dialog,
        Question,
        Answer,
        SetInt,
        CheckInt,
        IntVal,
        ItemGet,
        Equip,
        Store,
        SolidBG
    }
    public class EventNode
    {
        public EventNode parent, next;
        public List<EventNode> children;
        public NodeType type;

        public EventNode() {}

        public virtual void Process(GameBase game, GameplayManager gm, Player p, EventScript script)
        {

        }

        public virtual void Unload(GameBase game, GameplayManager gm, Player p, EventScript script)
        {

        }

        public void SetChildren(List<EventNode> children)
        {
            this.children = children;
        }
    }
}
