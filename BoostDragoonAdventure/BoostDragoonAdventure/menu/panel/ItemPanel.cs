using Microsoft.Xna.Framework;
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
        Item item;
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
    }
}
