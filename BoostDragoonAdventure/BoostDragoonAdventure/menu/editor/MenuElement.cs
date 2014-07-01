using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.editor.tool;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.display.sprite;

namespace wickedcrush.menu.editor
{
    public class MenuElement : MenuNode
    {
        public EditorTool tool;

        public MenuElement(TextSprite text, TextureSprite image, EditorTool tool, EditorMenu root) :
            base(text, image, root)
        {
            this.tool = tool;
        }
    }
}
