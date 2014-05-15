using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.factory.entity;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.utility.trigger;

namespace wickedcrush.entity.physics_entity.agent.trap.trigger
{
    public abstract class TriggerBase : Agent
    {
        protected Trigger trigger;

        public TriggerBase(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory)
            : base(w, pos, size, center, solid, factory)
        {
            trigger = new Trigger(this, new List<ITriggerable>(), c => c.isTriggered());
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            trigger.Update(gameTime);
        }

        public void clearWiring()
        {
            trigger.clearTargets();
        }

        public void connectWiring()
        {
            Stack<Body> openList = new Stack<Body>();
            Stack<Body> closedList = new Stack<Body>();

            Body curr;

            openList.Push(bodies["body"]);

            while (openList.Count > 0)
            {
                curr = openList.Peek();
                var c = curr.ContactList;

                while (c != null)
                {
                    if (c.Contact.IsTouching
                        && c.Other.UserData != null
                        && !c.Other.UserData.Equals(this))
                    {
                        if (c.Other.UserData is ITriggerable)
                        {
                            if(!closedList.Contains(c.Other))
                                trigger.addTarget((ITriggerable)c.Other.UserData);

                            if (!(openList.Contains(c.Other) || closedList.Contains(c.Other)))
                                openList.Push(c.Other);
                        }

                        if (c.Other.UserData is LayerType && ((LayerType)c.Other.UserData).Equals(LayerType.WIRING))
                        {
                            if (!(openList.Contains(c.Other) || closedList.Contains(c.Other)))
                                openList.Push(c.Other);
                        }
                    }

                    c = c.Next;
                }

                closedList.Push(openList.Pop());
            }
        }

        public abstract bool isTriggered();
    }
}
