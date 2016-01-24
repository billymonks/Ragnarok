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

        public bool center = true;
        public bool inScene = true;
        //public SpriteFont font;

        public String fontName = "Khula";

        public float maxWidth = -1;

        public Vector2 velocity = Vector2.Zero;

        public TextEntity(String text, Vector2 pos, SoundManager sound, GameBase g, int duration, EntityFactory factory, Color textColor, float zoomLevel, String fontName, bool addToScene)
            : this(text, pos, sound, g, duration, factory, textColor, zoomLevel, zoomLevel, addToScene)
        {
            this.fontName = fontName;
        }

        public TextEntity(String text, Vector2 pos, SoundManager sound, GameBase g, int duration, EntityFactory factory, Color textColor, float zoomLevel, float desiredZoomLevel, bool addToScene)
            : this(text, pos, sound, g, duration, factory, zoomLevel, addToScene)
        {
            this.desiredZoomLevel = desiredZoomLevel;
            this.textColor = textColor;
            
        }

        public TextEntity(String text, Vector2 pos, SoundManager sound, GameBase g, int duration, EntityFactory factory, float zoomLevel, bool addToScene) //duration time?
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

            if (addToScene)
            {
                factory._gm._screen.AddText(this);
            }

        }

        public TextEntity(String text, Vector2 pos, SoundManager sound, GameBase g, int duration, EntityFactory factory, float zoomLevel, float desiredZoomLevel, float zoomIncrement, bool addToScene) //duration time?
            : this(text, pos, sound, g, duration, factory, zoomLevel, addToScene)
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
            pos += velocity;
            zoomLevel = (zoomLevel + desiredZoomLevel) / 2f;
            desiredZoomLevel += zoomIncrement;

            
        }
        public override void Draw(bool depthPass, Dictionary<string, PointLightStruct> lightList, GameplayManager gameplay)
        {

            //Vector2 textPos = new Vector2(pos.X, pos.Y);

            //if(center)
                //g.spriteBatch.DrawString(g.fonts[fontName], text, textPos, textColor, 0f, g.fonts[fontName].MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0f);
            //else
                //g.spriteBatch.DrawString(g.fonts[fontName], text, textPos, textColor, 0f, Vector2.Zero, zoomLevel, SpriteEffects.None, 0f);
            //g.spriteBatch.DrawString(font, text, textPos + new Vector2(2, 2), textColor, 0f, font.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            //g.spriteBatch.DrawString(font, text, textPos - new Vector2(2, 2), textColor, 0f, font.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            //g.spriteBatch.DrawString(font, text, textPos + new Vector2(-2, 2), textColor, 0f, font.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            //g.spriteBatch.DrawString(font, text, textPos + new Vector2(2, -2), textColor, 0f, font.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0.001f);
            //g.spriteBatch.DrawString(font, text, textPos, textColor, 0f, font.MeasureString(text) / 2f, zoomLevel, SpriteEffects.None, 0f);

        }

        public void HudDraw(bool inScene, bool shadow)
        {
            Vector2 textPos;
            Vector2 origin;
            
            if(inScene)
                textPos = new Vector2((pos.X - factory._gm.camera.cameraPosition.X) * 2.25f, (pos.Y - factory._gm.camera.cameraPosition.Y) * 2.25f * (float)(Math.Sqrt(2) / 2));
            else
                textPos = new Vector2(pos.X, pos.Y);

            if (center)
            {
                origin = g.fonts[fontName].MeasureString(text) / 2f;
            }
            else
            {
                origin = Vector2.Zero;
            }


            if (shadow)
            {
                g.spriteBatch.DrawString(g.fonts[fontName], text, textPos + new Vector2(2, 2), Color.Black, 0f, origin, zoomLevel, SpriteEffects.None, 0.001f);
                g.spriteBatch.DrawString(g.fonts[fontName], text, textPos - new Vector2(2, 2), Color.Black, 0f, origin, zoomLevel, SpriteEffects.None, 0.001f);
                g.spriteBatch.DrawString(g.fonts[fontName], text, textPos + new Vector2(-2, 2), Color.Black, 0f, origin, zoomLevel, SpriteEffects.None, 0.001f);
                g.spriteBatch.DrawString(g.fonts[fontName], text, textPos + new Vector2(2, -2), Color.Black, 0f, origin, zoomLevel, SpriteEffects.None, 0.001f);
            }
            g.spriteBatch.DrawString(g.fonts[fontName], text, textPos, textColor, 0f, origin, zoomLevel, SpriteEffects.None, 0f);
        }

        public void SetMaxWidth(float width)
        {
            String tempText = "";
            float tempWidth = 0f;
            List<String> splitStrings = text.Split(' ').ToList<String>();
            
            maxWidth = width;
            
            foreach(String s in splitStrings) {
                if(tempWidth + g.fonts[fontName].MeasureString(s + ' ').X > width)
                {
                    tempText += "\n" + s + ' ';
                    tempWidth = g.fonts[fontName].MeasureString(s + ' ').X;
                } else {
                    tempText += s + ' ';
                    tempWidth += g.fonts[fontName].MeasureString(s + ' ').X;
                    if (s.Contains("\n"))
                    {
                        //tempWidth = g.fonts[fontName].MeasureString(s.Substring(s.IndexOf("\n"), s.Length - 1)).X;
                        tempWidth = 0f;
                    }
                }
            }


            text = tempText;
        }

        protected override void Dispose()
        {
            base.Dispose();

            factory._gm._screen.RemoveText(this);
        }
    }
}
