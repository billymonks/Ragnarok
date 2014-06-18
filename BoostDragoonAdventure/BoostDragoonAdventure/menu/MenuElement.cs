using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.menu
{
    public class MenuElement
    {
        public Vector2 pos, size;
        public float zoom = 1f;
        public String text;
        public Object value;
        public Menu subMenu = null;

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
