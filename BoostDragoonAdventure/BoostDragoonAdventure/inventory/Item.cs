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

        public bool pressReady = true, holdReady = false, releaseReady = false;

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



        public void Press(Agent a)
        {
            if (pressAction == null || !pressReady)
                return;

            foreach (KeyValuePair<string, int> req in pressRequirements)
            {
                if (!a.stats.requirementMet(req.Key, req.Value))
                {
                    return;
                }
            }

            if (a.stats.inventory.getItemCount(this) <= 0)
                return;


            pressAction(a, this);

            pressReady = false;
            holdReady = true;
            releaseReady = true;

            
        }

        public void Hold(Agent a)
        {
            if (holdAction == null || !holdReady)
                return;

            foreach (KeyValuePair<string, int> req in holdRequirements)
            {
                if (!a.stats.requirementMet(req.Key, req.Value))
                {
                    return;
                }
            }

            if (a.stats.inventory.getItemCount(this) <= 0)
                return;


            holdAction(a, this);

            pressReady = false;
            holdReady = true;
            releaseReady = true;
        }

        public void Release(Agent a)
        {
            if (releaseAction == null || !releaseReady)
                return;

            foreach (KeyValuePair<string, int> req in releaseRequirements)
            {
                if (!a.stats.requirementMet(req.Key, req.Value))
                {
                    return;
                }
            }

            if (a.stats.inventory.getItemCount(this) <= 0)
            {
                return;
            }

            releaseAction(a, this);

            pressReady = true;
            holdReady = false;
            releaseReady = false;
        }
    }
}
