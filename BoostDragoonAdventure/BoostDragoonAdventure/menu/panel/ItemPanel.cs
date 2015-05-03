﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.controls;
using wickedcrush.inventory;
using wickedcrush.display.primitives;

namespace wickedcrush.menu.panel
{
    public class ItemPanel : Panel
    {
        Inventory inventory;
        public Item item;
        public bool selected = false;
        String itemName = "";

        public ItemPanel(Color color, Point pos, Point size, Inventory inventory)
            : base(color, pos, size)
        {
            this.inventory = inventory;
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

            if (item != null)
            {
                itemName = item.name;

                if (inventory.itemA.Equals(item))
                {
                    PrimitiveDrawer.DrawFilledRectangle(sb, new Rectangle(pos.X + origin.X, pos.Y + origin.Y, 10, 10), Color.Red);
                    sb.DrawString(font, "J", new Vector2(pos.X + origin.X + 1, pos.Y + origin.Y - 1), Color.White);
                }

                if (inventory.itemB.Equals(item))
                {
                    PrimitiveDrawer.DrawFilledRectangle(sb, new Rectangle(pos.X + origin.X, pos.Y + origin.Y + 15, 10, 10), Color.Green);
                    sb.DrawString(font, "K", new Vector2(pos.X + origin.X + 1, pos.Y + origin.Y - 1 + 15), Color.White);
                }

                if (inventory.itemC.Equals(item))
                {
                    PrimitiveDrawer.DrawFilledRectangle(sb, new Rectangle(pos.X + origin.X, pos.Y + origin.Y + 30, 10, 10), Color.Blue);
                    sb.DrawString(font, "L", new Vector2(pos.X + origin.X + 1, pos.Y + origin.Y - 1 + 30), Color.White);
                }

                String itemCount = inventory.getItemCount(item).ToString();

                sb.DrawString(font, itemCount, new Vector2(pos.X + origin.X + size.X, pos.Y + origin.Y + size.Y) - font.MeasureString(itemCount), Color.White);

            }

            

            if(selected)
                sb.DrawString(font, itemName, new Vector2(pos.X + origin.X, pos.Y + origin.Y + size.Y), Color.White);
        }
    }
}
