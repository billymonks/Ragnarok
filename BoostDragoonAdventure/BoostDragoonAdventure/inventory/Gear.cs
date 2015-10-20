using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.inventory
{
    public class EquippedPart
    {
        public Part part;
        public Point pos;
        public int rotation;

        public EquippedPart(Part part, Point pos, int rotation)
        {
            this.part = part;
            this.pos = pos;
            this.rotation = rotation;
        }
    }

    public class Gear
    {
        EquippedPart core;
        List<EquippedPart> parts;

        bool changed = false; //re-calculate equipped stat value

        public Gear(EquippedPart core, List<EquippedPart> parts)
        {
            this.core = core;
            this.parts = parts;
        }
    }
}
