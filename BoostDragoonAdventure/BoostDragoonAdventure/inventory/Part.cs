using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
        public Dictionary<String, int> stats;

        public PartStruct(List<Point> slots, List<PartConnection> connections, Dictionary<String, int> stats)
        {
            this.slots = slots;
            this.connections = connections;
            this.stats = stats;
        }
    }

    public struct PartConnection
    {
        public Point slot, connectingSlot;
        public ConnectionType type;
        public bool female;

        public PartConnection(Point slot, Point connectingSlot, ConnectionType type, bool female)
        {
            this.slot = slot;
            this.connectingSlot = connectingSlot;
            this.type = type;
            this.female = female;
        }
    }

    public class Part : Item
    {
        PartStruct partStruct;
        public Part(String name, PartStruct partStruct)
            : base(name)
        {
            this.partStruct = partStruct;
        }
    }
}
