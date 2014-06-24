using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.display.sprite
{
    public abstract class BaseSprite
    {
        public Vector2 pos, origin;
        public float rotation;
        public Color color;
        public SpriteEffects spriteEffects = SpriteEffects.None;

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

        public abstract void Update();

        public abstract void DebugDraw();
    }
}
