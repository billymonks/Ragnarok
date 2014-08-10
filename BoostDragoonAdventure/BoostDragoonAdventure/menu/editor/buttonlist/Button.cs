using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.display.sprite;
using wickedcrush.screen;

namespace wickedcrush.menu.editor.buttonlist
{

    public delegate void ActionDelegate(Editor e);

    public class Button
    {
        public TextSprite text;
        public TextureSprite image;

        public Vector2 pos, size, center;
        public Rectangle hitbox;

        public ActionDelegate action;

        public bool highlighted = false;

        public Button(TextSprite text, TextureSprite image, ActionDelegate action)
        {
            this.text = text;
            this.image = image;
            this.action = action;

            pos = new Vector2(0, 0);
            size = new Vector2(100, 100);
            center = new Vector2(50, 50);
            hitbox = new Rectangle((int) (pos.X - center.X), (int) (pos.Y - center.Y), (int) size.X, (int) size.Y);
        }

        public void Update(GameTime gameTime, Vector2 cursorPosition)
        {
            UpdateSprites();
            UpdateHitbox();

            if (hitbox.Contains((int)cursorPosition.X, (int)cursorPosition.Y))
                highlighted = true;
            else
                highlighted = false;
        }

        private void UpdateHitbox()
        {
            hitbox.X = (int)(pos.X - center.X);
            hitbox.Y = (int)(pos.Y - center.Y);
            hitbox.Width = (int)size.X;
            hitbox.Height = (int)size.Y;
        }

        private void UpdateSprites()
        {
            image.setPos(pos.X, pos.Y);
            image.setSize(size.X, size.Y);
            text.setPos(pos.X, pos.Y);
        }

        public void Draw(SpriteBatch sb)
        {
            image.Draw(sb);
            text.Draw(sb);
        }

        public void runAction(Editor e)
        {
            action(e);
        }
    }
}
