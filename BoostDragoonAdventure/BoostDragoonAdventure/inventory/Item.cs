﻿using Microsoft.Xna.Framework;
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
        Unknown = 0,
        Consumable = 1,
        Part = 2
    }

    public class Item
    {
        public String name;
        public ItemType type = ItemType.Unknown;
        public int value = 10;

        public Item(String name)
        {
            this.name = name;
        }
    }
}
