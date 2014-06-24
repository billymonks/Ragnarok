using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.menu.editor
{
    public abstract class MenuNode
    {
        public String text;
        public Texture2D image;
        public MenuNode next, prev;

        public MenuNode(String text, Texture2D image)
        {
            this.text = text;
            this.image = image;
        }
    }
}
