using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.menu.editor.buttonlist;
using wickedcrush.screen;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.menu.editor
{
    public class EditorBarMenu
    {
        public Vector2 pos;

        private Vector2 cursorPosition;

        public List<Button> controlBar;

        public EditorScreen _editor;

        public EditorBarMenu(EditorScreen editor, Vector2 pos)
        {
            _editor = editor;
            this.pos = pos;

            controlBar = new List<Button>();
        }

        public void Update(GameTime gameTime, Vector2 cursor)
        {
            cursorPosition = cursor;

            UpdateBar(gameTime);
        }

        private void UpdateBar(GameTime gameTime)
        {

            for (int i = 0; i < controlBar.Count; i++)
            {
                controlBar[i].pos.X = pos.X + i * (controlBar[i].size.X + 10);
                controlBar[i].pos.Y = pos.Y;
                controlBar[i].Update(gameTime, cursorPosition);
            }
        }

        public void Click()
        {
            foreach (Button b in controlBar)
            {
                if (b.highlighted)
                    b.runAction(_editor);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Button b in controlBar)
                b.Draw(sb);
        }

        public void DebugDraw(SpriteBatch sb)
        {
            foreach (Button b in controlBar)
                b.Draw(sb);
        }
    }
}
