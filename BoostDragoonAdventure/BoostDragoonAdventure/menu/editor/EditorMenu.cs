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
        public List<MenuNode> nodes = new List<MenuNode>();
        bool isClicked = false;


        public EditorMenu()
        {
            
        }

        public EditorMenu(MenuNode node)
        {
            nodes.Add(node);
        }

        public void Update(GameTime gameTime)
        {
            isClicked = false;
        }

        public void DebugDraw(SpriteBatch sb)
        {
            MenuNode pointer;
            int j;

            for (int i = 0; i < nodes.Count; i++)
            {
                pointer = nodes[i];

                j = 0;

                while (pointer != null)
                {
                    pointer.image.setPos(100 + i * 52, 100 + j * 52);
                    pointer.image.Draw(sb);
                    pointer = pointer.next;
                    j++;
                }

                pointer = nodes[i].prev;
                j = 1;

                while (pointer != null)
                {
                    pointer.image.setPos(100 + i * 52, 100 - j * 52);
                    pointer.image.Draw(sb);
                    pointer = pointer.prev;
                    j++;
                }

            }
        }
    }
}
