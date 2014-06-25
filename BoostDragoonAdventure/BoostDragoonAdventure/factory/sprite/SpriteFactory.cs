using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using wickedcrush.display.sprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.factory.sprite
{
    public class SpriteFactory
    {
        private ContentManager cm;

        public SpriteFactory(ContentManager cm)
        {
            this.cm = cm;
        }

        public TextureSprite createTexture(String path, Vector2 pos, Vector2 origin, Vector2 size, Color color, float rotation)
        {
            TextureSprite sprite = new TextureSprite(pos, cm.Load<Texture2D>(path), origin, size, color, rotation);
            return sprite;
        }

        public TextSprite createText(Vector2 pos, String text, String fontPath, Vector2 scale, Vector2 origin, Color color, float rotation)
        {
            TextSprite sprite = new TextSprite(pos, text, cm.Load<SpriteFont>(fontPath), scale, origin, color, rotation);
            return sprite;
        }
    }
}
