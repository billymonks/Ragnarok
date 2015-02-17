using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.manager.audio;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.entity
{
    public class TextEntity : Entity
    {
        String text;
        GameBase g;

        public TextEntity(String text, Vector2 pos, SoundManager sound, GameBase g) //duration time?
            : base(pos, Vector2.Zero, Vector2.Zero, sound)
        {
            this.g = g;
            this.text = text;

        }
        public override void Draw()
        {
            g.spriteBatch.DrawString(g.testFont, text, pos, Color.White);
        }
    }
}
