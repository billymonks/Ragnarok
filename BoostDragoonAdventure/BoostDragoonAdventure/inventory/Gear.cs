using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.helper;
using wickedcrush.stats;

namespace wickedcrush.inventory
{

    public class EquippedPart
    {
        public Part part;

        public EquippedPart parentPart;
        public PartConnection parentConnection;

        public Point translation = new Point(0, 0);
        public int rotation = 0;

        public EquippedPart(Part part, EquippedPart parentPart, PartConnection parentConnection)
        {
            this.part = part;
            this.parentPart = parentPart;
            this.parentConnection = parentConnection;

            

            if (parentPart != null && parentConnection != null)
            {
                Point cRotation = new Point((int)Math.Round(Math.Cos(MathHelper.ToRadians(parentConnection.direction))), (int)Math.Round(Math.Sin(MathHelper.ToRadians(parentConnection.direction))));
                //Point cRotation = Point.Zero;

                rotation = (parentPart.rotation + parentConnection.direction) % 360;

                translation = TranslatePoint(RotatePoint(parentConnection.slot, parentPart.rotation), TranslatePoint(parentPart.translation, cRotation));
            }
        }

        public List<Point> GetEquippedSlots()
        {
            if (parentConnection == null)
                return part.partStruct.slots;

            List<Point> equippedSlots = new List<Point>();

            foreach (Point p in part.partStruct.slots)
            {
                equippedSlots.Add(TranslatePoint(RotatePoint(p, rotation), translation));
            }

            return equippedSlots;
        }

        private static Point RotatePoint(Point v, int rotation)
        {
            double ca = Math.Round(Math.Cos((rotation) * (Math.PI / 180)));
            double sa = Math.Round(Math.Sin((rotation) * (Math.PI / 180)));
            return new Point((int)(ca * v.X - sa * v.Y), (int)(sa * v.X + ca * v.Y));
        }

        private static Point TranslatePoint(Point v, Point pos)
        {
            return new Point(v.X + pos.X, v.Y + pos.Y);
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
            core = new EquippedPart(p, null, null);
        }

        public EquippedPart EquipPart(Part p, EquippedPart parentPart, PartConnection parentConnection)
        {
            EquippedPart tempPart = new EquippedPart(p, parentPart, parentConnection);

            parts.Add(tempPart);
            //if fits
            changed = true;

            return tempPart;
        }

        public void RemovePart(EquippedPart p)
        {
            changed = true;
        }

        public void RemoveAllParts()
        {
            parts.Clear();
            
            changed = true;
        }
    }
}
