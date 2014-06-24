using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace wickedcrush.menu
{
    public class EditorMenu
    {
        public MenuNode current;

        private Stack<MenuNode> selection;

        public EditorMenu()
        {

        }

        public void DebugDraw(SpriteBatch sb, SpriteFont f)
        {
            MenuNode pointer = current;
            int i = 0;

            do {
                sb.Draw(current.image, new Vector2(10, 10 + i*12), Color.Green);
                pointer = pointer.next;
                i++;
            } while (pointer != null && pointer != current);
        }
    }
}
