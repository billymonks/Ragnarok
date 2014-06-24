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
        public List<MenuNode> nodes;


        public EditorMenu()
        {
            CreateStandardMenu();
        }

        public EditorMenu(MenuNode node)
        {
            nodes.Add(node);
        }

        public void DebugDraw(SpriteBatch sb, SpriteFont f)
        {
            MenuNode pointer;
            int j;

            for (int i = 0; i < nodes.Count; i++)
            {
                pointer = nodes[i];

                j = 0;

                while (pointer != null)
                {
                    sb.Draw(pointer.image, new Vector2(10 + i * 12, 10 + j * 12), Color.Green);
                    pointer = pointer.next;
                    j++;
                }
            }
        }

        private void CreateStandardMenu()
        {
            
        }
    }
}
