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

        public Vector2 pos, size, center;
        public Rectangle hitbox;

        public Queue<Tween> posXTweenQueue = new Queue<Tween>(),
            posYTweenQueue = new Queue<Tween>(), sizeXTweenQueue = new Queue<Tween>(), sizeYTweenQueue = new Queue<Tween>(),
            centerXTweenQueue = new Queue<Tween>(), centerYTweenQueue = new Queue<Tween>(),
            colorATweenQueue = new Queue<Tween>(), colorRTweenQueue = new Queue<Tween>(), colorGTweenQueue = new Queue<Tween>(), colorBTweenQueue = new Queue<Tween>();

        public bool visible = false;

        public MenuNode(TextSprite text, TextureSprite image)
        {
            this.text = text;
            this.image = image;

            pos = new Vector2(0, 0);
            size = new Vector2(100, 100);
            center = new Vector2(50, 50);
            hitbox = new Rectangle((int) (pos.X - center.X), (int) (pos.Y - center.Y), (int) size.X, (int) size.Y);
        }

        public void Update(GameTime gameTime)
        {
            if (!visible && sizeXTweenQueue.Count == 0 && centerXTweenQueue.Count == 0 && centerYTweenQueue.Count == 0 && colorATweenQueue.Count == 0)
            {
                tweenSize(10, 10, 100);
                tweenColor(0, 0, 0, 0, 100);
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

                pos.Y = posYTweenQueue.Peek().getValue();

                if (posYTweenQueue.Peek().finished)
                {
                    posYTweenQueue.Dequeue().Remove();
                }
            }

            if (sizeXTweenQueue.Count > 0)
            {
                sizeXTweenQueue.Peek().Update(gameTime);

                size.X = sizeXTweenQueue.Peek().getValue();

                if (sizeXTweenQueue.Peek().finished)
                {
                    sizeXTweenQueue.Dequeue().Remove();
                }
            }

            if (sizeYTweenQueue.Count > 0)
            {
                sizeYTweenQueue.Peek().Update(gameTime);

                size.Y = sizeYTweenQueue.Peek().getValue();

                if (sizeYTweenQueue.Peek().finished)
                {
                    sizeYTweenQueue.Dequeue().Remove();
                }
            }

            if (centerXTweenQueue.Count > 0)
            {
                centerXTweenQueue.Peek().Update(gameTime);

                center.X = centerXTweenQueue.Peek().getValue();

                if (centerXTweenQueue.Peek().finished)
                {
                    centerXTweenQueue.Dequeue().Remove();
                }
            }

            if (centerYTweenQueue.Count > 0)
            {
                centerYTweenQueue.Peek().Update(gameTime);

                center.Y = centerYTweenQueue.Peek().getValue();

                if (centerYTweenQueue.Peek().finished)
                {
                    centerYTweenQueue.Dequeue().Remove();
                }
            }

            if (colorATweenQueue.Count > 0)
            {
                colorATweenQueue.Peek().Update(gameTime);

                image.color.A = (byte) colorATweenQueue.Peek().getValue();

                if (colorATweenQueue.Peek().finished)
                {
                    colorATweenQueue.Dequeue().Remove();
                }
            }

            if (colorRTweenQueue.Count > 0)
            {
                colorRTweenQueue.Peek().Update(gameTime);

                image.color.R = (byte)colorRTweenQueue.Peek().getValue();

                if (colorRTweenQueue.Peek().finished)
                {
                    colorRTweenQueue.Dequeue().Remove();
                }
            }

            if (colorGTweenQueue.Count > 0)
            {
                colorGTweenQueue.Peek().Update(gameTime);

                image.color.G = (byte)colorGTweenQueue.Peek().getValue();

                if (colorGTweenQueue.Peek().finished)
                {
                    colorGTweenQueue.Dequeue().Remove();
                }
            }

            if (colorBTweenQueue.Count > 0)
            {
                colorBTweenQueue.Peek().Update(gameTime);

                image.color.B = (byte)colorBTweenQueue.Peek().getValue();

                if (colorBTweenQueue.Peek().finished)
                {
                    colorBTweenQueue.Dequeue().Remove();
                }
            }
        }

        private void UpdateHitbox()
        {
            hitbox.X = (int) (pos.X - center.X);
            hitbox.Y = (int) (pos.Y - center.Y);
            hitbox.Width = (int) size.X;
            hitbox.Height = (int) size.Y;
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

        public void tweenColor(byte r, byte g, byte b, byte a, int ms)
        {
            colorRTweenQueue.Enqueue(new Tween((float)image.color.R, (float)r, new TimeSpan(0, 0, 0, 0, ms)));
            colorGTweenQueue.Enqueue(new Tween((float)image.color.G, (float)g, new TimeSpan(0, 0, 0, 0, ms)));
            colorBTweenQueue.Enqueue(new Tween((float)image.color.B, (float)b, new TimeSpan(0, 0, 0, 0, ms)));
            colorATweenQueue.Enqueue(new Tween((float)image.color.A, (float)a, new TimeSpan(0, 0, 0, 0, ms)));
        }
    }
}
