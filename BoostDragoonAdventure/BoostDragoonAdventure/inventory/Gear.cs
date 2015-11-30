using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.helper;
using wickedcrush.stats;

namespace wickedcrush.inventory
{
    public class EquippedConnection
    {
        public EquippedPart parent, child;
        public PartConnection connection;

        public Point translation, cRotation;

        public EquippedConnection(EquippedPart part, PartConnection connection)
        {
            this.parent = part;
            this.connection = connection;

            cRotation = new Point((int)Math.Round(Math.Cos(MathHelper.ToRadians(connection.direction))), (int)Math.Round(Math.Sin(MathHelper.ToRadians(connection.direction))));
            cRotation = Helper.RotatePoint(cRotation, parent.rotation);
            //Point cRotation = Point.Zero;
            //translation = Helper.TranslatePoint(Helper.RotatePoint(connection.slot, parent.rotation), Helper.TranslatePoint(parent.translation, cRotation));
            translation = Helper.TranslatePoint(Helper.RotatePoint(connection.slot, parent.rotation), parent.translation);
        }
    }
    public class EquippedPart
    {
        public Part part;
        public List<EquippedConnection> equippedConnections;

        public EquippedPart parentPart;
        public PartConnection parentConnection;

        public Point translation = new Point(0, 0);
        public int rotation = 0;

        public EquippedPart(Part part, EquippedConnection parent)
        {
            this.part = part;

            equippedConnections = new List<EquippedConnection>();

            

            if (parent != null)
            {
                this.parentPart = parent.parent;
                this.parentConnection = parent.connection;

                rotation = (parentPart.rotation + parentConnection.direction) % 360;

                Point cRotation = new Point((int)Math.Round(Math.Cos(MathHelper.ToRadians(rotation))), (int)Math.Round(Math.Sin(MathHelper.ToRadians(rotation))));
                //Point cRotation = Point.Zero;

                

                translation = Helper.TranslatePoint(Helper.RotatePoint(parentConnection.slot, parentPart.rotation), Helper.TranslatePoint(parentPart.translation, cRotation));

                
                parent.child = this;


            }

            foreach (PartConnection partConnect in part.partStruct.connections)
            {
                EquippedConnection tempConnection = new EquippedConnection(this, partConnect);

                equippedConnections.Add(tempConnection);
            }
        }

        public List<Point> GetEquippedSlots()
        {
            if (parentConnection == null)
                return part.partStruct.slots;

            List<Point> equippedSlots = new List<Point>();

            foreach (Point p in part.partStruct.slots)
            {
                equippedSlots.Add(Helper.TranslatePoint(Helper.RotatePoint(p, rotation), translation));
            }

            return equippedSlots;
        }

        
    }

    public class Gear
    {
        public EquippedPart core;
        public List<EquippedPart> parts;
        public int frameSize = 3; // 7x7

        bool changed = false; //re-calculate equipped stat value

        public Gear(EquippedPart core, List<EquippedPart> parts)
        {
            this.core = core;
            this.parts = parts;
        }

        public void UpdateStats(PersistedStats stats)
        {
            changed = false;
        }

        public void EquipCore(Part p)
        {
            core = new EquippedPart(p, null);
        }

        public EquippedPart EquipPart(Part p, EquippedConnection parentConnection)
        {
            EquippedPart tempPart = new EquippedPart(p, parentConnection);

            parts.Add(tempPart);
            //if part doesn't fit game will crash so have a great day
            changed = true;

            return tempPart;
        }

        public int GetGearStat(GearStat stat)
        {
            int value = 0;
            foreach (EquippedPart equippedPart in parts)
            {
                foreach (KeyValuePair<GearStat, int> pair in equippedPart.part.partStruct.stats)
                {
                    if (pair.Key == stat)
                    {
                        value += pair.Value;
                    }
                }
            }

            return value;
        }

        public bool PartFits(Part part, EquippedConnection connection)
        {
            bool fits = true;
            if (part.partStruct.connections[0].female || part.partStruct.connections[0].type != connection.connection.type)
                return false;

            EquippedPart tempPart = new EquippedPart(part, connection);

            List<Point> occupiedSlots = GetAllEquippedSlots();

            foreach (Point point in tempPart.GetEquippedSlots())
            {
                if (Math.Abs(point.X) > frameSize || Math.Abs(point.Y) > frameSize || occupiedSlots.Contains(point))
                    fits = false;
            }

            

            connection.child = null;

            return fits;
        }

        public EquippedPart GetPreviewPart(Part part, EquippedConnection connection)
        {

            EquippedPart tempPart = new EquippedPart(part, connection);


            connection.child = null;

            return tempPart;
        }

        public Dictionary<GearStat, int> GetEquippedStats()
        {
            Dictionary<GearStat, int> result = new Dictionary<GearStat, int>();

            foreach (EquippedPart part in parts)
            {
                foreach (KeyValuePair<GearStat, int> pair in part.part.partStruct.stats)
                {
                    if (result.ContainsKey(pair.Key))
                    {
                        result[pair.Key] = result[pair.Key] + pair.Value;
                    }
                    else
                    {
                        result.Add(pair.Key, pair.Value);
                    }
                }
            }

            return result;
        }

        public List<Point> GetAllEquippedSlots()
        {
            List<Point> slotList = new List<Point>();
            foreach (EquippedPart part in parts)
            {
                foreach (Point point in part.GetEquippedSlots())
                {
                    slotList.Add(point);
                }
            }

            foreach (Point point in core.GetEquippedSlots())
            {
                slotList.Add(point);
            }

            return slotList;
        }

        public void RemovePart(EquippedPart p)
        {
            foreach (EquippedConnection connection in p.equippedConnections)
            {
                if (!connection.connection.female)
                {
                    

                    foreach (EquippedConnection parentConnection in connection.parent.parentPart.equippedConnections)
                    {
                        if (parentConnection.child == p)
                        {
                            parentConnection.child = null;
                        }
                    }

                    //connection.parent = null;
                    
                }
            }

            RemoveNestedParts(p);

            changed = true;
        }

        private void RemoveNestedParts(EquippedPart p)
        {
            foreach (EquippedConnection connection in p.equippedConnections)
            {
                if (connection.connection.female && connection.child != null)
                {
                    RemoveNestedParts(connection.child);
                }
                //else if (connection.connection.female && connection.parent != null)
                //{
                    //connection.parent = null;
                //}
            }

            if (parts.Contains(p))
                parts.Remove(p);

            changed = true;
        }

        public void RemoveAllParts()
        {
            parts.Clear();
            
            changed = true;
        }
    }
}
