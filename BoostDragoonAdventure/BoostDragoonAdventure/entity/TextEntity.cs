using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.manager.audio;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.utility;
using wickedcrush.factory.entity;
using wickedcrush.display._3d;
using wickedcrush.manager.gameplay;

namespace wickedcrush.entity
{
    public class TextEntity : Entity
    {
        public String text;
        GameBase g;
        Timer duration;
        EntityFactory factory;

        public float zoomLevel = 1f;
        public float desiredZoomLevel = 1f;
        public float zoomIncrement = 0.0f;

        public Color textColor = Color.White;
        //public SpriteFont font;

        public String fontName = "Khula";

        public TextEntity(String text, Vector2 pos, SoundManager sound, GameBase g, int duration, EntityFactory factory, Color textColor, float zoomLevel, String fontName)
            : this(text, pos, sound, g, duration, factory, textColor, zoomLevel)
        {
            this.fontName = fontName;
        }

        public TextEntity(String text, Vector2 pos, SoundManager sound, GameBase g, int duration, EntityFactory factory, Color textColor, float zoomLevel)
            : this(text, pos, sound, g, duration, factory, zoomLevel)
        {
            this.textColor = textColor;
            
        }

        public TextEntity(String text, Vector2 pos, SoundManager sound, GameBase g, int duration, EntityFactory factory, float zoomLevel) //duration time?
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
            this.zoomLevel = zoomLevel;
            //font = g.fonts[fontName];

            factory._gm._screen.AddText(this);

        }

        public TextEntity(String text, Vector2 pos, SoundManager sound, GameBase g, int duration, EntityFactory factory, float zoomLevel, float desiredZoomLevel, float zoomIncrement) //duration time?
            : this(text, pos, sound, g, duration, factory, zoomLevel)
        {
            this.zoomLevel = zoomLevel;
            this.desiredZoomLevel = desiredZoomLevel;
            this.zoomIncrement = zoomIncrement;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (duration != null)
            {
                duration.Update(gameTime);
                if (duration.isDone())
                {
                    Remove();
                }
            }

            zoomLevel = (zoomLevel + desiredZoomLevel) / 2f;
            desiredZoomLevel += zoomIncrement;

            
        }
        public override void Draw(bool depthPass, Dictionary<string, PointLightStruct> lightList, GameplayManager gameplay)
        {

            //Vector2 textPos = new Vector2((pos.X - factory._gm.camera.cameraPosition.X) * 2.25f, (pos.Y - factory._gm.camera.cameraPosition.Y) * 2.25f * (float)(Math.Sqrt(2) / 2));


            //g.spriteBatch.DrawString(font, text, textPos + new Vector2(2, 2), textColor, 0f, font.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            //g.spriteBatch.DrawString(font, text, textPos - new Vector2(2, 2), textColor, 0f, font.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            //g.spriteBatch.DrawString(font, text, textPos + new Vector2(-2, 2), textColor, 0f, font.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            //g.spriteBatch.DrawString(font, text, textPos + new Vector2(2, -2), textColor, 0f, font.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            //g.spriteBatch.DrawString(font, text, textPos, textColor, 0f, font.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0f);

        }

        public void HudDraw()
        {
            Vector2 textPos = new Vector2((pos.X - factory._gm.camera.cameraPosition.X) * 2.25f, (pos.Y - factory._gm.camera.cameraPosition.Y) * 2.25f * (float)(Math.Sqrt(2) / 2));


            g.spriteBatch.DrawString(g.fonts[fontName], text, textPos + new Vector2(2, 2), Color.Black, 0f, g.fonts[fontName].MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            g.spriteBatch.DrawString(g.fonts[fontName], text, textPos - new Vector2(2, 2), Color.Black, 0f, g.fonts[fontName].MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            g.spriteBatch.DrawString(g.fonts[fontName], text, textPos + new Vector2(-2, 2), Color.Black, 0f, g.fonts[fontName].MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            g.spriteBatch.DrawString(g.fonts[fontName], text, textPos + new Vector2(2, -2), Color.Black, 0f, g.fonts[fontName].MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            g.spriteBatch.DrawString(g.fonts[fontName], text, textPos, textColor, 0f, g.fonts[fontName].MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0f);
        }

        protected override void Dispose()
        {
            base.Dispose();

            factory._gm._screen.RemoveText(this);
        }
    }
}
