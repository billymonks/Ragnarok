using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.menu
{
    public class MenuElement : MenuNode
    {
        public Object value;

        public MenuElement()
        {

        }

        public MenuElement(String text, Vector2 pos, Vector2 size)
        {
            this.text = text;
            this.pos = pos;
            this.size = size;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void DebugDraw()
        {

        }
    }
}
