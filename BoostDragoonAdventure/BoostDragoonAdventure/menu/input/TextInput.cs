using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.controls;
using wickedcrush.display.primitives;

namespace wickedcrush.menu.input
{
    public class TextInput
    {
        private KeyboardControls controls;
        private String input;

        public bool finished = false, cancelled = false;

        public TextInput(KeyboardControls controls)
        {
            this.controls = controls;
            input = "";
        }

        public void Update(GameTime gameTime)
        {
            //controls.Update();

            if (controls.GetInput().Contains("\b"))
            {
                if (input.Length > 0)
                    input = input.Remove(input.Length - 1);
            }
            else
                input += controls.GetInput();

            if (controls.StartPressed())
                finished = true;

            if (controls.SelectPressed())
                cancelled = true;
        }

        public String getText()
        {
            return input;
        }

        public void DebugDraw(SpriteBatch sb, SpriteFont f)
        {
            if (!PrimitiveDrawer.isInitialized())
            {
                PrimitiveDrawer.LoadContent(sb.GraphicsDevice);
            }

            if (!(finished || cancelled))
            {
                sb.DrawFilledRectangle(new Rectangle(100, 100, 300, 50), Color.White);
                sb.DrawString(f, input, new Vector2(100, 100), Color.Black);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (!PrimitiveDrawer.isInitialized())
            {
                PrimitiveDrawer.LoadContent(sb.GraphicsDevice);
            }

            if(!(finished||cancelled))
                sb.DrawFilledRectangle(new Rectangle(100, 100, 300, 50), Color.White);
            
        }
    }
}
