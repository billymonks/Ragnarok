using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using wickedcrush.entity.physics_entity.agent;
using wickedcrush.entity.physics_entity.agent.trap.trigger;
using wickedcrush.entity.physics_entity.agent.trap.triggerable.turret;
using wickedcrush.entity.physics_entity.agent.trap.triggerable;

namespace wickedcrush.map.circuit
{
    public class Circuit
    {
        List<Body> wiring;

        List<Triggerable> triggerables = new List<Triggerable>();
        List<TriggerBase> triggers = new List<TriggerBase>();

        public Circuit()
        {
            wiring = new List<Body>();
        }

        public void addWiring(Body wire)
        {
            if (!wiring.Contains(wire))
                wiring.Add(wire);
        }

        public bool contains(Body wire)
        {
            return wiring.Contains(wire);
        }

        public void connectTriggers()
        {
            foreach (Body b in wiring)
            {
                
                var c = b.ContactList;

                while (c != null)
                {
                    if (c.Contact.IsTouching
                        && c.Other.UserData != null
                        && !c.Other.UserData.Equals(this))
                    {
                        if (c.Other.UserData is Triggerable)
                        {
                            if (!triggerables.Contains(c.Other.UserData))
                                triggerables.Add((Triggerable)c.Other.UserData);
                        }
                        else if (c.Other.UserData is TriggerBase)
                        {
                            if (!triggers.Contains(c.Other.UserData))
                                triggers.Add((TriggerBase)c.Other.UserData);
                        }
                    }

                    c = c.Next;
                }
            }

            foreach (TriggerBase t in triggers)
            {
                t.trigger.addTargets(triggerables);
            }
        }
    }
}
