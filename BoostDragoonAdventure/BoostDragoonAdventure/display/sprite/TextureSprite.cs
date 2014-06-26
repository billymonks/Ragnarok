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
        public Vector2 size;

        private Rectangle rectangle = new Rectangle();

        public TextureSprite(Vector2 pos, Texture2D texture)
            : base(pos)
        {
            this.texture = texture;
            this.size = new Vector2(texture.Width, texture.Height);

            UpdateRectangle();
        }

        public TextureSprite(Vector2 pos, Texture2D texture, Vector2 origin, Vector2 size, Color color, float rotation)
            : base(pos, origin, color, rotation)
        {
            this.texture = texture;
            this.size = size;

            UpdateRectangle();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateRectangle();
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, rectangle, null, color, rotation, origin, spriteEffects, 0f);
        }

        private void UpdateRectangle()
        {
            rectangle.X = (int)pos.X;
            rectangle.Y = (int)pos.Y;
            rectangle.Width = (int)size.X;
            rectangle.Height = (int)size.Y;
        }
    }
}
