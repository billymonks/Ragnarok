using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent;
using wickedcrush.controls;

namespace wickedcrush.inventory
{
    public delegate void ItemAction(Agent a, Item i);

    public enum ItemType
    {
        Consumable = 0,
        UsesFuel = 1,
        UsesAmmo = 2,
        UsesFuelCharge = 3
    }

    public class Item
    {
        public String name;
        protected ItemAction pressAction, holdAction, releaseAction;
        public ItemType type;

        public List<KeyValuePair<string, int>> pressRequirements;
        public List<KeyValuePair<string, int>> holdRequirements;
        public List<KeyValuePair<string, int>> releaseRequirements;

        

        public Item(
            String name,
            ItemAction pressAction,
            ItemAction holdAction,
            ItemAction releaseAction,
            List<KeyValuePair<string, int>> pressRequirements,
            List<KeyValuePair<string, int>> holdRequirements,
            List<KeyValuePair<string, int>> releaseRequirements)
        {
            this.name = name;
            this.pressAction = pressAction;
            this.holdAction = holdAction;
            this.releaseAction = releaseAction;

            this.pressRequirements = pressRequirements;
            this.holdRequirements = holdRequirements;
            this.releaseRequirements = releaseRequirements;
        }

        public Item(Item i)
        {
            this.name = i.name;
            this.pressAction = i.pressAction;
            this.holdAction = i.holdAction;
            this.releaseAction = i.releaseAction;

            this.pressRequirements = i.pressRequirements;
            this.holdRequirements = i.holdRequirements;
            this.releaseRequirements = i.releaseRequirements;
        }



        public void Press(Agent a)
        {
            if (pressAction == null || !a.pressReady || a.itemInUse != null)
                return;

            a.itemInUse = this;

            foreach (KeyValuePair<string, int> req in pressRequirements)
            {
                if (!a.stats.requirementMet(req.Key, req.Value))
                {
                    a.itemInUse = null;
                    return;
                }
            }

            if (a.stats.inventory.getItemCount(this) <= 0)
                return;

            a.pressReady = false;
            a.holdReady = true;
            a.releaseReady = true;

            pressAction(a, this);

            

            
        }

        public void Hold(Agent a)
        {
            if (holdAction == null || !a.holdReady || (a.itemInUse != this && a.itemInUse != null))
                return;

            a.itemInUse = this;

            foreach (KeyValuePair<string, int> req in holdRequirements)
            {
                if (!a.stats.requirementMet(req.Key, req.Value))
                {
                    a.itemInUse = null;
                    return;
                }
            }

            if (a.stats.inventory.getItemCount(this) <= 0)
                return;

            a.pressReady = false;
            a.holdReady = true;
            a.releaseReady = true;

            holdAction(a, this);

            
        }

        public void Release(Agent a)
        {
            if (releaseAction == null || !a.releaseReady || (a.itemInUse != this && a.itemInUse != null))
                return;

            foreach (KeyValuePair<string, int> req in releaseRequirements)
            {
                if (!a.stats.requirementMet(req.Key, req.Value))
                {
                    a.itemInUse = null;
                    return;
                }
            }

            if (a.stats.inventory.getItemCount(this) <= 0)
            {
                return;
            }

            a.pressReady = true;
            a.holdReady = false;
            a.releaseReady = false;

            releaseAction(a, this);

            
        }

        public void Cancel(Agent a)
        {
            if (a.itemInUse == this)
                a.itemInUse = null;

            a.pressReady = true;
            a.holdReady = false;
            a.releaseReady = false;
        }
    }
}
