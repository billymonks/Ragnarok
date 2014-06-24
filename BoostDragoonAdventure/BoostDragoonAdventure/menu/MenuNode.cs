using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.menu
{
    public abstract class MenuNode
    {
        public String text;
        public Texture2D image;

        public MenuNode next, prev;
    }
}
