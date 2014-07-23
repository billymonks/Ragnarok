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

        public Point pos, size, center;
        public Rectangle hitbox;

        public Queue<Tween> posXTweenQueue = new Queue<Tween>(),
            posYTweenQueue = new Queue<Tween>(), sizeXTweenQueue = new Queue<Tween>(), sizeYTweenQueue = new Queue<Tween>(),
            centerXTweenQueue = new Queue<Tween>(), centerYTweenQueue = new Queue<Tween>();

        public bool visible = false;

        public MenuNode(TextSprite text, TextureSprite image)
        {
            this.text = text;
            this.image = image;

            pos = new Point(0, 0);
            size = new Point(50, 50);
            center = new Point(25, 25);
            hitbox = new Rectangle(pos.X - center.X, pos.Y - center.Y, size.X, size.Y);
        }

        public void Update(GameTime gameTime)
        {
            if (!visible && sizeXTweenQueue.Count == 0 && centerXTweenQueue.Count == 0 && centerYTweenQueue.Count == 0)
            {
                tweenSize(10, 10, 100);
            }

            text.Update(gameTime);
            image.Update(gameTime);

            UpdateTweens(gameTime);

            UpdateHitbox();
            UpdateSprites();

            

            visible = false;
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

            if (sizeXTweenQueue.Count > 0)
            {
                sizeXTweenQueue.Peek().Update(gameTime);

                size.X = (int)sizeXTweenQueue.Peek().getValue();

                if (sizeXTweenQueue.Peek().finished)
                {
                    sizeXTweenQueue.Dequeue().Remove();
                }
            }

            if (sizeYTweenQueue.Count > 0)
            {
                sizeYTweenQueue.Peek().Update(gameTime);

                size.Y = (int)sizeYTweenQueue.Peek().getValue();

                if (sizeYTweenQueue.Peek().finished)
                {
                    sizeYTweenQueue.Dequeue().Remove();
                }
            }

            if (centerXTweenQueue.Count > 0)
            {
                centerXTweenQueue.Peek().Update(gameTime);

                center.X = (int)centerXTweenQueue.Peek().getValue();

                if (centerXTweenQueue.Peek().finished)
                {
                    centerXTweenQueue.Dequeue().Remove();
                }
            }

            if (centerYTweenQueue.Count > 0)
            {
                centerYTweenQueue.Peek().Update(gameTime);

                center.Y = (int)centerYTweenQueue.Peek().getValue();

                if (centerYTweenQueue.Peek().finished)
                {
                    centerYTweenQueue.Dequeue().Remove();
                }
            }
        }

        private void UpdateHitbox()
        {
            hitbox.X = pos.X - center.X;
            hitbox.Y = pos.Y - center.Y;
            hitbox.Width = size.X;
            hitbox.Height = size.Y;
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

        public void tweenPosition(int x, int y, int ms)
        {
            posXTweenQueue.Enqueue(new Tween((float)pos.X, (float)x, new TimeSpan(0, 0, 0, 0, ms)));
            posYTweenQueue.Enqueue(new Tween((float)pos.Y, (float)y, new TimeSpan(0, 0, 0, 0, ms)));
        }

        public void tweenSize(int x, int y, int ms)
        {
            sizeXTweenQueue.Enqueue(new Tween((float)size.X, (float)x, new TimeSpan(0, 0, 0, 0, ms)));
            sizeYTweenQueue.Enqueue(new Tween((float)size.Y, (float)y, new TimeSpan(0, 0, 0, 0, ms)));

            centerXTweenQueue.Enqueue(new Tween((float)center.X, ((float)center.X / (float)size.X) * (float)x, new TimeSpan(0, 0, 0, 0, ms)));
            centerYTweenQueue.Enqueue(new Tween((float)center.Y, ((float)center.Y / (float)size.Y) * (float)y, new TimeSpan(0, 0, 0, 0, ms)));
        }
    }
}
