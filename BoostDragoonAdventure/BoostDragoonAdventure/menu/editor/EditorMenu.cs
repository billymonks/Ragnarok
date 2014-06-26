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


        public EditorMenu()
        {
            CreateStandardMenu();
        }

        public EditorMenu(MenuNode node)
        {
            nodes.Add(node);
        }

        public void Update(GameTime gameTime)
        {
            MenuNode pointer;
            int j;

            for (int i = 0; i < nodes.Count; i++)
            {
                pointer = nodes[i];

                j = 0;
                while (pointer != null)
                {
                    pointer.Update(gameTime);
                    pointer = pointer.next;
                    j++;
                }
            }
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
                    pointer.image.pos.X = 100 + i * 52;
                    pointer.image.pos.Y = 100 + j * 52;
                    pointer.image.Draw(sb);
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
