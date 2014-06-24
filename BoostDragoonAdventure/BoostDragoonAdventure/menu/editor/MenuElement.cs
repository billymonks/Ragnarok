using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.editor.tool;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.menu.editor
{
    public class MenuElement : MenuNode
    {
        public EditorTool tool;

        public MenuElement(String text, Texture2D image, EditorTool tool) :
            base(text, image)
        {
            this.tool = tool;
        }
    }
}
