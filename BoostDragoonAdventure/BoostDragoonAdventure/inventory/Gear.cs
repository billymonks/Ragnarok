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
        public PartConnection parentConnection;

        public EquippedPart(Part part, PartConnection parentConnection)
        {
            this.part = part;
            this.parentConnection = parentConnection;
        }

        public List<Point> GetEquippedSlots()
        {
            if (parentConnection == null)
                return part.partStruct.slots;

            List<Point> equippedSlots = new List<Point>();

            foreach (Point p in part.partStruct.slots)
            {
                equippedSlots.Add(TranslatePoint(RotatePoint(p, parentConnection.direction), parentConnection.slot));
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
        EquippedPart core;
        List<EquippedPart> parts;
        int frameSize = 3; // 7x7

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

        public void EquipPart(Part p, PartConnection parentConnection)
        {
            EquippedPart tempPart = new EquippedPart(p, parentConnection);

            //if fits
            changed = true;
        }

        public void RemovePart(EquippedPart p)
        {
            changed = true;
        }
    }
}
