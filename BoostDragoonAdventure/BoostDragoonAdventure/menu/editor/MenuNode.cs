using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.display.sprite;
using wickedcrush.display.animation;

namespace wickedcrush.menu.editor
{
    public abstract class MenuNode
    {
        public TextSprite text;
        public TextureSprite image;
        public MenuNode parent, next, prev;

        public Point pos, size;
        public Rectangle hitbox;

        public Queue<Tween> posXTweenQueue = new Queue<Tween>(),
            posYTweenQueue = new Queue<Tween>();

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

            UpdateTweens(gameTime);

            UpdateHitbox();
            UpdateSprites();
        }

        private void UpdateTweens(GameTime gameTime)
        {
            if (posXTweenQueue.Count > 0)
            {
                posXTweenQueue.Peek().Update(gameTime);

                pos.X = (int)posXTweenQueue.Peek().getValue();

                if (posXTweenQueue.Peek().finished)
                {
                    posXTweenQueue.Dequeue().Remove();
                }
            }

            if (posYTweenQueue.Count > 0)
            {
                posYTweenQueue.Peek().Update(gameTime);

                pos.Y = (int)posYTweenQueue.Peek().getValue();

                if (posYTweenQueue.Peek().finished)
                {
                    posYTweenQueue.Dequeue().Remove();
                }
            }
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

        public void tweenPosition(int x, int y, int ms)
        {
            posXTweenQueue.Enqueue(new Tween((float)pos.X, (float)x, new TimeSpan(0, 0, 0, 0, ms)));
            posYTweenQueue.Enqueue(new Tween((float)pos.Y, (float)y, new TimeSpan(0, 0, 0, 0, ms)));
        }
    }
}
