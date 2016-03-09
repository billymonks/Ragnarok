using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent;
using wickedcrush.controls;

namespace wickedcrush.inventory
{
    public delegate void WeaponAction(Agent a, Weapon i);

    public class Weapon : Item
    {
        protected WeaponAction pressAction, holdAction, releaseAction, equipAction, unequipAction;

        public List<KeyValuePair<string, int>> pressRequirements;
        public List<KeyValuePair<string, int>> holdRequirements;
        public List<KeyValuePair<string, int>> releaseRequirements;

        public Weapon(
            String name,
            String desc,
            WeaponAction pressAction,
            WeaponAction holdAction,
            WeaponAction releaseAction,
            WeaponAction equipAction,
            WeaponAction unequipAction,
            List<KeyValuePair<string, int>> pressRequirements,
            List<KeyValuePair<string, int>> holdRequirements,
            List<KeyValuePair<string, int>> releaseRequirements)
            : base(name, desc)
        {
            this.pressAction = pressAction;
            this.holdAction = holdAction;
            this.releaseAction = releaseAction;
            this.equipAction = equipAction;
            this.unequipAction = unequipAction;

            this.pressRequirements = pressRequirements;
            this.holdRequirements = holdRequirements;
            this.releaseRequirements = releaseRequirements;
        }

        public Weapon(
            String name,
            String desc,
            WeaponAction pressAction,
            WeaponAction holdAction,
            WeaponAction releaseAction,
            List<KeyValuePair<string, int>> pressRequirements,
            List<KeyValuePair<string, int>> holdRequirements,
            List<KeyValuePair<string, int>> releaseRequirements) : base(name, desc)
        {
            this.pressAction = pressAction;
            this.holdAction = holdAction;
            this.releaseAction = releaseAction;
            this.equipAction = (a, i) =>
               { };
            this.unequipAction = (a, i) =>
               { };

            this.pressRequirements = pressRequirements;
            this.holdRequirements = holdRequirements;
            this.releaseRequirements = releaseRequirements;

            this.value = 2500;
        }

        public Weapon(Weapon i) : base(i.name, i.desc)
        {
            this.pressAction = i.pressAction;
            this.holdAction = i.holdAction;
            this.releaseAction = i.releaseAction;

            this.pressRequirements = i.pressRequirements;
            this.holdRequirements = i.holdRequirements;
            this.releaseRequirements = i.releaseRequirements;
        }



        public void Press(Agent a)
        {
            if (pressAction == null || !a.pressReady || a.weaponInUse != null || a.overheating)
                return;

            a.weaponInUse = this;

            foreach (KeyValuePair<string, int> req in pressRequirements)
            {
                if (!a.stats.requirementMet(req.Key, req.Value))
                {
                    a.weaponInUse = null;
                    return;
                }
            }

            if (a.stats.inventory.getItemCount(this) <= 0)
                return;

            a.pressReady = false;
            a.holdReady = true;
            a.releaseReady = true;

            pressAction(a, this);

            unequipAction(a, this);

            
        }

        public void Hold(Agent a)
        {
            if (holdAction == null || !a.holdReady || (a.weaponInUse != this && a.weaponInUse != null))
                return;

            a.weaponInUse = this;

            foreach (KeyValuePair<string, int> req in holdRequirements)
            {
                if (!a.stats.requirementMet(req.Key, req.Value))
                {
                    a.weaponInUse = null;
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
            if (releaseAction == null || !a.releaseReady || (a.weaponInUse != this && a.weaponInUse != null))
                return;

            foreach (KeyValuePair<string, int> req in releaseRequirements)
            {
                if (!a.stats.requirementMet(req.Key, req.Value))
                {
                    a.weaponInUse = null;
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
            unequipAction(a, this);
            
        }

        public void Cancel(Agent a)
        {
            if (a.weaponInUse == this)
                a.weaponInUse = null;

            a.pressReady = true;
            a.holdReady = false;
            a.releaseReady = false;

            unequipAction(a, this);
        }

        public void Equip(Agent a)
        {
            equipAction(a, this);
        }
    }
}
