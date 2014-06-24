using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.menu.editor
{
    public class SubMenu : MenuNode
    {
        public MenuNode current;

        public SubMenu(String text, Texture2D image, MenuNode current) 
            : base(text, image)
        {
            this.current = current;
        }
    }
}
