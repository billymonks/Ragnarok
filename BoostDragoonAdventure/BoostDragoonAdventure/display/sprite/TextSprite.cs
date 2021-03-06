﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace wickedcrush.display.sprite
{
    public class TextSprite : BaseSprite
    {
        public String text;
        public SpriteFont font;
        public Vector2 scale;

        public bool shadow = true, visible = true;

        public TextSprite(Vector2 pos, String text, SpriteFont font, Vector2 scale, Vector2 origin, Color color, float rotation)
            : base(pos, origin, color, rotation)
        {
            this.text = text;
            this.font = font;
            this.scale = scale;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch sb)
        {
            if (!visible)
                return;

            if (shadow)
            {
                sb.DrawString(font, text, getPos() + new Vector2(1, 1), Color.Black, rotation, origin, scale, spriteEffects, 0f);
                sb.DrawString(font, text, getPos() + new Vector2(-1, -1), Color.Black, rotation, origin, scale, spriteEffects, 0f);
                sb.DrawString(font, text, getPos() + new Vector2(1, -1), Color.Black, rotation, origin, scale, spriteEffects, 0f);
                sb.DrawString(font, text, getPos() + new Vector2(-1, 1), Color.Black, rotation, origin, scale, spriteEffects, 0f);
            }

            sb.DrawString(font, text, getPos(), color, rotation, origin, scale, spriteEffects, 0f);
        }

    }
}
