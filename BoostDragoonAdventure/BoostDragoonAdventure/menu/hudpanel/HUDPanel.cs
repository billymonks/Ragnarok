using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.display.primitives;

namespace wickedcrush.menu.hudpanel
{
    public class HUDPanel
    {
        public Color color;
        public Rectangle rectangle;

        public HUDPanel parent;
        public List<HUDPanel> children = new List<HUDPanel>();

        public HUDPanel(Color color, Rectangle rectangle)
        {
            this.color = color;
            this.rectangle = rectangle;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void DebugDraw(SpriteBatch sb, Point origin)
        {
            rectangle.X += origin.X;
            rectangle.Y += origin.Y;

            foreach(HUDPanel h in children)
            { h.DebugDraw(sb, origin); }

            PrimitiveDrawer.DrawFilledRectangle(sb, rectangle, color);

            rectangle.X -= origin.X;
            rectangle.Y -= origin.Y;
        }
    }
}
