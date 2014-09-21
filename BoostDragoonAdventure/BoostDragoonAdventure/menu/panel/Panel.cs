using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.controls;
using wickedcrush.display.primitives;

namespace wickedcrush.menu.panel
{
    public class Panel
    {
        public Color color;
        public Point pos, size;

        private Rectangle r;

        public Panel parent;
        public Dictionary<string, Panel> children = new Dictionary<string, Panel>();

        public Panel(Color color, Point pos, Point size)
        {
            this.color = color;
            this.pos = pos;
            this.size = size;

            r = new Rectangle(pos.X, pos.Y, size.X, size.Y);
        }

        public virtual void Update(GameTime gameTime, Controls controls)
        {
            UpdateControls(controls);
        }

        public void DebugDraw(SpriteBatch sb, Point origin)
        {
            r.X = pos.X + origin.X;
            r.Y = pos.Y + origin.Y;

            PrimitiveDrawer.DrawFilledRectangle(sb, r, color);

            foreach (KeyValuePair<string, Panel> pair in children)
            { pair.Value.DebugDraw(sb, origin); }

        }

        public virtual void UpdateControls(Controls controls)
        {
            
        }
    }
}
