using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace wickedcrush.menu.editor
{
    public class EditorMenu
    {
        public MenuNode current;
        public Vector2 pos = new Vector2(100, 300);

        bool isClicked = false;


        public EditorMenu()
        {
            
        }

        public EditorMenu(MenuNode node)
        {
            current = node;
        }

        public void Update(GameTime gameTime)
        {
            isClicked = false;
        }

        public void DebugDraw(SpriteBatch sb)
        {
            MenuNode pointer;

            //draw current
            pointer = current;

            DrawStem(sb, pointer, 0);

            //draw children if submenu
            if (current is SubMenu)
            {
                pointer = ((SubMenu)pointer).current;

                DrawStem(sb, pointer, 1);
            }

            //draw parents
            pointer = current;

            while (pointer.parent != null)
            {
                pointer = current.parent;

                DrawStem(sb, pointer, -1);
            }
        }

        private void DrawStem(SpriteBatch sb, MenuNode pointer, int i)
        {
            int j = 0;
            MenuNode memory = pointer;

            //next
            while (pointer != null)
            {
                pointer.image.setPos(pos.X + i * 52, pos.Y + j * 52);
                pointer.image.Draw(sb);

                pointer = pointer.next;
                j++;
            }

            pointer = memory.prev;
            j = -1;

            //prev
            while (pointer != null)
            {
                pointer.image.setPos(pos.X + i * 52, pos.Y + j * 52);
                pointer.image.Draw(sb);

                pointer = pointer.prev;
                j--;
            }
        }
    }
}
