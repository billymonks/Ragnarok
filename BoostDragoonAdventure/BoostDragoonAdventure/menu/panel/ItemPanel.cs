using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.controls;
using wickedcrush.inventory;

namespace wickedcrush.menu.panel
{
    public class ItemPanel : Panel
    {
        public Item item;
        public bool selected = false;

        public ItemPanel(Color color, Point pos, Point size)
            : base(color, pos, size)
        {

        }

        public void setItem(Item item)
        {
            this.item = item;
        }

        public override void Update(GameTime gameTime, Controls controls)
        {
            base.Update(gameTime, controls);
        }

        public override void DebugDraw(SpriteBatch sb, Point origin, SpriteFont font)
        {
            base.DebugDraw(sb, origin, font);
            if(selected)
                sb.DrawString(font, item.name, new Vector2(pos.X + origin.X, pos.Y + origin.Y + size.Y), Color.White);
        }
    }
}
