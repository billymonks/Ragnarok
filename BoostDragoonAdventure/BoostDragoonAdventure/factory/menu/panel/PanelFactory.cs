using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.menu.panel;

namespace wickedcrush.factory.menu.panel
{
    public class PanelFactory
    {
        public PanelFactory()
        {

        }

        public Panel getInventory()
        {
            Panel root = new Panel(Color.DarkSlateGray, new Point(-250, -100), new Point(190, 220));

            root.children.Add("slot1", new Panel(Color.SlateGray, new Point(-240, -90), new Point(50, 50)));
            root.children.Add("slot2", new Panel(Color.SlateGray, new Point(-180, -90), new Point(50, 50)));
            root.children.Add("slot3", new Panel(Color.SlateGray, new Point(-120, -90), new Point(50, 50)));

            root.children.Add("slot4", new Panel(Color.SlateGray, new Point(-240, -30), new Point(50, 50)));
            root.children.Add("slot5", new Panel(Color.SlateGray, new Point(-180, -30), new Point(50, 50)));
            root.children.Add("slot6", new Panel(Color.SlateGray, new Point(-120, -30), new Point(50, 50)));

            root.children.Add("slot7", new Panel(Color.SlateGray, new Point(-240, 30), new Point(50, 50)));
            root.children.Add("slot8", new Panel(Color.SlateGray, new Point(-180, 30), new Point(50, 50)));
            root.children.Add("slot9", new Panel(Color.SlateGray, new Point(-120, 30), new Point(50, 50)));

            root.children.Add("commands", new Panel(Color.SlateGray, new Point(-240, 90), new Point(170, 20)));

            return root;
        }
    }
}
