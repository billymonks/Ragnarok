using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.manager.audio;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.utility;
using wickedcrush.factory.entity;

namespace wickedcrush.entity
{
    public class TextEntity : Entity
    {
        String text;
        GameBase g;
        Timer duration;
        EntityFactory factory;

        float zoomLevel = 1f;
        float desiredZoomLevel = 2f;

        public TextEntity(String text, Vector2 pos, SoundManager sound, GameBase g, int duration, EntityFactory factory) //duration time?
            : base(pos, Vector2.Zero, Vector2.Zero, sound)
        {
            this.g = g;
            this.text = text;
            if (duration > 0)
            {
                this.duration = new Timer(duration);
                this.duration.resetAndStart();
            }

            this.factory = factory;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            duration.Update(gameTime);
            if (duration.isDone())
            {
                Remove();
            }

            zoomLevel = (zoomLevel + desiredZoomLevel) / 2f;
            desiredZoomLevel += 0.01f;

            
        }
        public override void Draw(bool depthPass)
        {

            Vector2 textPos = new Vector2((pos.X - factory._gm.camera.cameraPosition.X) * 2.25f, (pos.Y - factory._gm.camera.cameraPosition.Y) * 2.25f * (float)(Math.Sqrt(2) / 2));

            
            g.spriteBatch.DrawString(g.testFont, text, textPos + new Vector2(2, 2), Color.Black, 0f, g.testFont.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            g.spriteBatch.DrawString(g.testFont, text, textPos - new Vector2(2, 2), Color.Black, 0f, g.testFont.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            g.spriteBatch.DrawString(g.testFont, text, textPos + new Vector2(-2, 2), Color.Black, 0f, g.testFont.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            g.spriteBatch.DrawString(g.testFont, text, textPos + new Vector2(2, -2), Color.Black, 0f, g.testFont.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            g.spriteBatch.DrawString(g.testFont, text, textPos, Color.White, 0f, g.testFont.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0f);

        }
    }
}
