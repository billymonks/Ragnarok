using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.controls;
using wickedcrush.inventory;

namespace wickedcrush.menu.panel
{
    public class InventoryPanel : Panel
    {
        Inventory inventory;
        List<Item> itemList;
        int i = 0, row = 0, col = 0, rowScroll = 0;

        public InventoryPanel(Inventory inventory) : base(Color.DarkSlateGray, new Point(-250, -100), new Point(190, 220))
        {
            this.inventory = inventory;

            children.Add("slot0", new ItemPanel(Color.SlateGray, new Point(-240, -90), new Point(50, 50)));
            children.Add("slot1", new ItemPanel(Color.SlateGray, new Point(-180, -90), new Point(50, 50)));
            children.Add("slot2", new ItemPanel(Color.SlateGray, new Point(-120, -90), new Point(50, 50)));
            children.Add("slot3", new ItemPanel(Color.SlateGray, new Point(-240, -30), new Point(50, 50)));
            children.Add("slot4", new ItemPanel(Color.SlateGray, new Point(-180, -30), new Point(50, 50)));
            children.Add("slot5", new ItemPanel(Color.SlateGray, new Point(-120, -30), new Point(50, 50)));
            children.Add("slot6", new ItemPanel(Color.SlateGray, new Point(-240, 30), new Point(50, 50)));
            children.Add("slot7", new ItemPanel(Color.SlateGray, new Point(-180, 30), new Point(50, 50)));
            children.Add("slot8", new ItemPanel(Color.SlateGray, new Point(-120, 30), new Point(50, 50)));
            children.Add("commands", new Panel(Color.SlateGray, new Point(-240, 90), new Point(170, 20)));

            itemList = inventory.GetItemList();
        }

        public override void Update(GameTime gameTime, Controls controls)
        {
            
            int selection;

            if (inventory.changed)
                itemList = inventory.GetItemList();

            UpdateControls(controls);

            if (i > itemList.Count - 1)
                i = itemList.Count - 1;

            if (i < 0)
                i = 0;

            row = i / 3;
            col = i % 3;

            if (rowScroll > row)
                rowScroll = row;

            if (row > rowScroll + 2)
                rowScroll = row - 2;

            selection = (row * 3) + col;

            setAllInactive();
            highlightItems();
            highlightSelection();
        }

        private void setAllInactive()
        {
            for (int j = 0; j < 9; j++)
            {
                children["slot" + j].color = Color.DarkSlateGray;
                ((ItemPanel)children["slot" + j]).setItem(null);
                ((ItemPanel)children["slot" + j]).selected = false;
            }
        }

        private void highlightItems()
        {
            for(int j = 0; j < 9; j++)
            {
                if (itemList.Count > (rowScroll * 3) + j)
                {
                    children["slot" + j].color = Color.SlateGray;
                    ((ItemPanel)children["slot" + j]).setItem(itemList[(rowScroll * 3) + j]);
                }
            }
        }

        private void highlightSelection()
        {
            for (int j = 0; j < 9; j++)
            {
                if (i == (rowScroll * 3) + j)
                {
                    children["slot" + j].color = Color.LightGray;
                    ((ItemPanel)children["slot" + j]).selected = true;
                }
            }
        }

        public override void UpdateControls(Controls controls)
        {
            if (controls.ItemAPressed())
            {
                inventory.itemA = itemList[i];
            }

            if (controls.ItemBPressed())
            {
                inventory.itemB = itemList[i];
            }

            if (controls.ItemCPressed())
            {
                inventory.itemC = itemList[i];
            }

            if(controls.UpPressed())
                i-=3;

            if (controls.DownPressed())
                i+=3;

            if (controls.LeftPressed())
                i--;

            if (controls.RightPressed())
                i++;

            
        }
    }
}
