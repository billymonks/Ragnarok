using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.display.sprite;

namespace wickedcrush.menu.editor
{
    public class SubMenu : MenuNode
    {
        public MenuNode current;

        public SubMenu(TextSprite text, TextureSprite image, MenuNode current, EditorMenu root) 
            : base(text, image, root)
        {
            this.current = current;
            this.current.parent = this;
        }
    }
}
