using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.display.sprite;

namespace wickedcrush.menu.editor
{
    public abstract class MenuNode
    {
        public TextSprite text;
        public TextureSprite image;
        public MenuNode parent, next, prev;

        public Point pos, size;
        public Rectangle hitbox;

        public MenuNode(TextSprite text, TextureSprite image)
        {
            this.text = text;
            this.image = image;

            pos = new Point(0, 0);
            size = new Point(50, 50);
            hitbox = new Rectangle(pos.X, pos.Y, size.X, size.Y);
        }

        public void Update(GameTime gameTime)
        {
            text.Update(gameTime);
            image.Update(gameTime);

            UpdateHitbox();
            UpdateSprites();
        }

        private void UpdateHitbox()
        {
            hitbox.X = pos.X;
            hitbox.Y = pos.Y;
            hitbox.Width = size.X;
            hitbox.Height = size.Y;
        }

        private void UpdateSprites()
        {
            image.setPos(pos.X, pos.Y);
            text.setPos(pos.X, pos.Y);
        }

        public void Draw(SpriteBatch sb)
        {
            image.Draw(sb);
            text.Draw(sb);
        }
    }
}
