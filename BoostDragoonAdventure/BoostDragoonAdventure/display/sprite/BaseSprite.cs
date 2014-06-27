using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.display.sprite
{
    public abstract class BaseSprite
    {
        private Vector2 pos;

        public Vector2 origin; //origin is pre-scale/resize
        public float rotation;
        public Color color;
        public SpriteEffects spriteEffects = SpriteEffects.None;

        protected bool remove = false;

        public BaseSprite(Vector2 pos)
        {
            this.pos = pos;
            this.origin = new Vector2(0f, 0f);
            color = Color.White;
            rotation = 0f;
        }

        public BaseSprite(Vector2 pos, Vector2 origin, Color color, float rotation)
        {
            this.pos = pos;
            this.origin = origin;
            this.color = color;
            this.rotation = rotation;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch sb);

        public virtual void Remove()
        {
            remove = true;
        }

        public bool readyForRemoval()
        {
            return remove;
        }

        public Vector2 getPos()
        {
            return pos;
        }

        public virtual void setPos(float x, float y)
        {
            pos.X = x;
            pos.Y = y;
        }
    }
}
