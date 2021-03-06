﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.stats;

namespace wickedcrush.inventory
{
    public enum ConnectionType
    {
        Square = 0,
        Circle = 1,
        Triangle = 2
    }

    public struct PartStruct
    {
        public List<Point> slots;
        public List<PartConnection> connections;
        public Dictionary<GearStat, int> stats;
        public bool core;

        public PartStruct(List<Point> slots, List<PartConnection> connections, Dictionary<GearStat, int> stats)
        {
            this.slots = slots;
            this.connections = connections;
            this.stats = stats;
            this.core = false;
        }

        public PartStruct(List<Point> slots, List<PartConnection> connections, Dictionary<GearStat, int> stats, bool core)
        {
            this.slots = slots;
            this.connections = connections;
            this.stats = stats;
            this.core = core;
        }
    }

    public class PartConnection
    {
        public Point slot;
        public int direction;
        public ConnectionType type;
        public bool female;

        public PartConnection(Point slot, int direction, ConnectionType type, bool female)
        {
            this.slot = slot;
            this.direction = direction;
            this.type = type;
            this.female = female;
        }
    }

    public class Part : Item
    {
        public PartStruct partStruct;
        public Part(String name, PartStruct partStruct)
            : base(name, "This part can be equipped at a garage.")
        {
            this.partStruct = partStruct;
        }
    }

    public class Core : Part
    {
        public Core(String name, PartStruct partStruct)
            : base(name, partStruct)
        {
            this.partStruct.core = true;
        }
    }
}
