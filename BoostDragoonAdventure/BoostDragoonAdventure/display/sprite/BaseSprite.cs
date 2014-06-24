using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.display.sprite
{
    public class BaseSprite
    {
        Vector2 pos;
        float alpha, rotation;

        public BaseSprite(Vector2 pos)
        {
            this.pos = pos;
            alpha = 1f;
            rotation = 0f;
        }

        public BaseSprite(Vector2 pos, float alpha, float rotation)
        {
            this.pos = pos;
            this.alpha = alpha;
            this.rotation = rotation;
        }
    }
}
