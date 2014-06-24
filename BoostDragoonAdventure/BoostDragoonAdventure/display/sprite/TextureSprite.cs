using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace wickedcrush.display.sprite
{
    public class TextureSprite : BaseSprite
    {
        public Texture2D texture;

        TextureSprite(Vector2 pos, Texture2D texture)
            : base(pos)
        {
            this.texture = texture;
        }

        TextureSprite(Vector2 pos, Texture2D texture, float alpha, float rotation) : base(pos, alpha, rotation)
        {
            this.texture = texture;
        }
    }
}
