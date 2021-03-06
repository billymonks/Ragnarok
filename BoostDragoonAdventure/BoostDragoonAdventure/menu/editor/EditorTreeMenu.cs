﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using wickedcrush.editor.tool;
using wickedcrush.display.primitives;
using wickedcrush.menu.editor.buttonlist;
using wickedcrush.screen;

namespace wickedcrush.menu.editor
{
    public class EditorTreeMenu
    {
        public MenuNode current;
        public Vector2 pos;

        private Vector2 cursorPosition;

        public MenuNode highlighted;

        public EditorTool tool;

        public Dictionary<string, MenuNode> nodes;

        

        public EditorScreen _editor;

        public EditorTreeMenu(EditorScreen editor, Dictionary<string, MenuNode> nodes, Vector2 pos)
        {
            _editor = editor;
            this.pos = pos;

            SetNodes(nodes);
        }

        public void SetNodes(Dictionary<string, MenuNode> nodes)
        {
            this.nodes = nodes;
            foreach (KeyValuePair<string, MenuNode> pair in nodes)
            {
                current = pair.Value;
                break;
            }
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
            if (nodes == null)
                return;

            foreach(KeyValuePair<string, MenuNode> pair in nodes)
            {
                pair.Value.Update(gameTime);
            }
        }

        public void SetCurrentToTool(EditorTool tool)
        {
            foreach (KeyValuePair<string, MenuNode> node in nodes)
            {
                if (node.Value is MenuElement && ((MenuElement)node.Value).tool.Equals(tool))
                {
                    current = node.Value;
                }
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
            if (!PrimitiveDrawer.isInitialized())
            {
                PrimitiveDrawer.LoadContent(sb.GraphicsDevice);
            }
            //sb.DrawCircle(pos, 27, Color.LightBlue, 8, 32);
            //sb.DrawCircle(pos, 35, Color.LightGreen, 8, 32);

            foreach (KeyValuePair<string, MenuNode> pair in nodes)
            {
                pair.Value.Draw(sb);
            }

            sb.DrawCircle(pos, 30, Color.LightPink, 1, 32);
            
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
            {
                int xPos, yPos;

                if (node.parent == null)
                {
                    xPos = (int)(pos.X + i * 62);
                }
                else
                {
                    xPos = (int)(pos.X + i * node.parent.size.X);
                }
                    
                yPos = (int)(pos.Y + j * node.size.Y);

                node.tweenPosition(xPos, yPos, 100);
            }

            //node.Update(gameTime);

            if (node.hitbox.Contains((int)cursorPosition.X, (int)cursorPosition.Y))
                highlighted = node;

            if (node.colorATweenQueue.Count == 0)
            {
                Color c = generateColor(i, j, node.Equals(highlighted));
                node.tweenColor(c.R, c.G, c.B, c.A, 100);
            }

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

            //if (node.colorATweenQueue.Count == 0)
                //node.tweenColor(255, 100);

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
