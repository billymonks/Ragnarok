﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using wickedcrush.editor.tool;

namespace wickedcrush.menu.editor
{
    public class EditorMenu
    {
        public MenuNode current;
        public Vector2 pos = new Vector2(150, 300);

        private Vector2 cursorPosition;

        public MenuNode highlighted;

        public EditorTool tool;

        public Dictionary<string, MenuNode> nodes;


        public EditorMenu()
        {
            nodes = new Dictionary<string, MenuNode>();
        }

        public EditorMenu(Dictionary<string, MenuNode> nodes)
        {
            this.nodes = nodes;

            foreach (KeyValuePair<string, MenuNode> pair in nodes)
            {
                current = pair.Value;
                break;
            }
        }

        public EditorMenu(Dictionary<string, MenuNode> nodes, MenuNode node)
        {
            this.nodes = nodes;
            current = node;
        }

        public void Update(GameTime gameTime, Vector2 cursor)
        {
            cursorPosition = cursor;

            highlighted = null;

            UpdateVisible(gameTime);
            UpdateNodes(gameTime);
        }

        private void UpdateNodes(GameTime gameTime)
        {
            foreach(KeyValuePair<string, MenuNode> pair in nodes)
            {
                pair.Value.Update(gameTime);
            }
        }

        public void Click()
        {
            if(highlighted!=null)
                current = highlighted;
        }

        public EditorTool currentTool()
        {
            if (current is MenuElement)
                return ((MenuElement)current).tool;

            else return null;
        }

        private void UpdateVisible(GameTime gameTime)
        {
            MenuNode pointer;

            //draw current
            pointer = current;

            UpdateStem(gameTime, pointer, 0);

            //draw children if submenu
            if (current is SubMenu)
            {
                pointer = ((SubMenu)pointer).current;

                UpdateStem(gameTime, pointer, 1);
            }

            //draw parents
            pointer = current;

            if (pointer != null && pointer.parent != null)
            {
                pointer = current.parent;

                UpdateStem(gameTime, pointer, -1);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (KeyValuePair<string, MenuNode> pair in nodes)
            {
                pair.Value.Draw(sb);
            }
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

            if (pointer != null && pointer.parent != null)
            {
                pointer = current.parent;

                DrawStem(sb, pointer, -1);
            }
        }

        private void DrawStem(SpriteBatch sb, MenuNode pointer, int i)
        {
            if (pointer == null)
                return;

            int j = 0;
            MenuNode memory = pointer;

            //next
            while (pointer != null)
            {
                pointer.Draw(sb);

                pointer = pointer.next;
                j++;
            }

            pointer = memory.prev;
            j = -1;

            //prev
            while (pointer != null)
            {
                pointer.Draw(sb);

                pointer = pointer.prev;
                j--;
            }
        }

        private void UpdateStem(GameTime gameTime, MenuNode pointer, int i)
        {
            if (pointer == null)
                return;

            int j = 0;
            MenuNode memory = pointer;

            //next
            while (pointer != null)
            {
                UpdateVisibleNode(gameTime, pointer, i, j);

                pointer = pointer.next;
                j++;
            }

            pointer = memory.prev;
            j = -1;

            //prev
            while (pointer != null)
            {
                UpdateVisibleNode(gameTime, pointer, i, j);

                pointer = pointer.prev;
                j--;
            }
        }

        private void UpdateVisibleNode(GameTime gameTime, MenuNode node, int i, int j)
        {
            //node.pos.X = (int)pos.X + i * 52;
            //node.pos.Y = (int)pos.Y + j * 52;

            if (node.posXTweenQueue.Count == 0 && node.posYTweenQueue.Count == 0)
                node.tweenPosition((int)pos.X + i * 52, (int)pos.Y + j * 52, 100);

            //node.Update(gameTime);

            if (node.hitbox.Contains((int)cursorPosition.X, (int)cursorPosition.Y))
                highlighted = node;

            node.image.color = generateColor(i, j, node.Equals(highlighted));

            if (node.Equals(highlighted))
            {
                node.text.visible = true;
                node.text.color = Color.Yellow;
            }
            else if (node.Equals(current) || node.Equals(current.parent))
            {
                node.text.visible = true;
                node.text.color = Color.White;
            } 
            else
            {
                node.text.visible = false;
            }

            if (node.sizeXTweenQueue.Count == 0 && node.sizeYTweenQueue.Count == 0)
            {
                node.tweenSize(50 - (i * 20), 50 - (i * 20), 100);
            }

            node.visible = true;
        }

        private Color generateColor(int i, int j, bool highlighted)
        {
            i = Math.Abs(i);
            j = Math.Abs(j);

            float a = 2f, b = 3f;

            Color c = new Color( a / (i + j + b), a / (i + j + b), a / (i + j + b));

            if (highlighted)
                c.G += 30;

            return c;
        }
    }
}
