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
    public enum TextAlignment
    {
        Left,
        Right,
        Center
    }

    public class TextEntity : Entity
    {
        public String text, displayedText;
        GameBase g;
        Timer duration;
        EntityFactory factory;

        public float zoomLevel = 1f;
        public float desiredZoomLevel = 1f;
        public float zoomIncrement = 0.0f;

        public Color textColor = Color.White;

        //public bool center = true;
        public TextAlignment alignment = TextAlignment.Center;
        public bool inScene = true, instant = false, shadow = false;
        //public SpriteFont font;

        public String fontName = "Khula";

        public float maxWidth = -1;

        public Vector2 velocity = Vector2.Zero;

        public Timer dialogTimer;
        private int shortTime = 15;
        private int longTime = 70;
        public int lastIndex = 0;

        public String cueName = "";

        Random random;
        int instanceCount = 0;

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
            random = new Random();
            this.g = g;
            this.text = text;
            this.displayedText = "";

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
                inScene = true;
                factory._gm._screen.AddText(this);
            } else {
                inScene = false;
            }

            dialogTimer = new Timer(shortTime);
            dialogTimer.resetAndStart();

        }

        public TextEntity(String text, Vector2 pos, SoundManager sound, GameBase g, int duration, EntityFactory factory, float zoomLevel, float desiredZoomLevel, float zoomIncrement, bool addToScene) //duration time?
            : this(text, pos, sound, g, duration, factory, zoomLevel, addToScene)
        {
            this.zoomLevel = zoomLevel;
            this.desiredZoomLevel = desiredZoomLevel;
            this.zoomIncrement = zoomIncrement;
        }

        public void ChangeText(String text)
        {
            if (instant)
            {
                this.text = text;
                displayedText = text;
            } else if (!this.text.Equals(text))
            {
                dialogTimer.resetAndStart();
                this.text = text;
                displayedText = "";
                lastIndex = 0;
            }
            else
            {
                dialogTimer.start();
            }
        }

        public void SetColor(Color color)
        {
            this.textColor = color;
        }

        public void SetSpeed(int length)
        {
            if (length <= 0)
            {
                instant = true;
                displayedText = text;
            }
            dialogTimer.setInterval(1);
        }

        public void ChangeText(String text, float width)
        {

                String tempText = "";
                float tempWidth = 0f;
                List<String> splitStrings = text.Split(' ').ToList<String>();

                maxWidth = width;

                foreach (String s in splitStrings)
                {
                    if (tempWidth + g.fonts[fontName].MeasureString(s + ' ').X > width)
                    {
                        tempText += "\n" + s + ' ';
                        tempWidth = g.fonts[fontName].MeasureString(s + ' ').X;
                    }
                    else
                    {
                        tempText += s + ' ';
                        tempWidth += g.fonts[fontName].MeasureString(s + ' ').X;
                        if (s.Contains("\n"))
                        {
                            //tempWidth = g.fonts[fontName].MeasureString(s.Substring(s.IndexOf("\n"), s.Length - 1)).X;
                            tempWidth = 0f;
                        }
                    }
                }




                if (!this.text.Equals(tempText))
                {
                    dialogTimer.resetAndStart();
                    this.text = tempText;
                    displayedText = "";
                    lastIndex = 0;
                }
                else
                {
                    dialogTimer.start();
                }
        }

        public void AppendText(String text)
        {
            dialogTimer.resetAndStart();
            this.text += text;

            if (instant)
            {
                this.displayedText = text;
            }
            //displayedText = "";
            //lastIndex = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            dialogTimer.Update(gameTime);

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

            if (instant)
            {
                displayedText = text;
            }
            else if (lastIndex < text.Length && dialogTimer.isDone())
            {
                if (cueName != "" && text[lastIndex] != ' ')
                {
                    //_sound.playCue(cueName);
                    _sound.addCueInstance("Jump7", "Jump7" + this.id + "-" + instanceCount, false);
                    _sound.setCueVariable("PitchIncrease", MathHelper.Lerp(0f, 100f, (float)(random.NextDouble())), "Jump7" + this.id + instanceCount);
                    _sound.playCueInstance("Jump7" + this.id + "-" + instanceCount++);
                }
                dialogTimer.resetAndStart();
                lastIndex++;
                displayedText = text.Substring(0, lastIndex);

            } 

            
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

        public void HudDraw(bool inScene, bool outline)
        {
            Vector2 textPos;
            Vector2 origin;
            
            if(inScene)
                textPos = new Vector2((pos.X - factory._gm.camera.cameraPosition.X) * 2.25f, (pos.Y - factory._gm.camera.cameraPosition.Y) * 2.25f * (float)(Math.Sqrt(2) / 2));
            else
                textPos = new Vector2(pos.X, pos.Y);

            if (alignment == TextAlignment.Center)
            {
                origin = g.fonts[fontName].MeasureString(displayedText) / 2f;
            }
            else if (alignment == TextAlignment.Left)
            {
                origin = Vector2.Zero;
            }
            else
            {
                origin = g.fonts[fontName].MeasureString(displayedText);
            }


            if (shadow || outline)
            {
                g.spriteBatch.DrawString(g.fonts[fontName], displayedText, textPos + new Vector2(2, 2), Color.Black, 0f, origin, zoomLevel, SpriteEffects.None, 0.001f);
                g.spriteBatch.DrawString(g.fonts[fontName], displayedText, textPos - new Vector2(2, 2), Color.Black, 0f, origin, zoomLevel, SpriteEffects.None, 0.001f);
                g.spriteBatch.DrawString(g.fonts[fontName], displayedText, textPos + new Vector2(-2, 2), Color.Black, 0f, origin, zoomLevel, SpriteEffects.None, 0.001f);
                g.spriteBatch.DrawString(g.fonts[fontName], displayedText, textPos + new Vector2(2, -2), Color.Black, 0f, origin, zoomLevel, SpriteEffects.None, 0.001f);
            }
            g.spriteBatch.DrawString(g.fonts[fontName], displayedText, textPos, textColor, 0f, origin, zoomLevel, SpriteEffects.None, 0f);
        }

        public void SetMaxWidth(float width)
        {
            if (maxWidth != width)
            {
                String tempText = "";
                float tempWidth = 0f;
                List<String> splitStrings = text.Split(' ').ToList<String>();

                maxWidth = width;

                foreach (String s in splitStrings)
                {
                    if (tempWidth + g.fonts[fontName].MeasureString(s + ' ').X > width)
                    {
                        tempText += "\n" + s + ' ';
                        tempWidth = g.fonts[fontName].MeasureString(s + ' ').X;
                    }
                    else
                    {
                        tempText += s + ' ';
                        tempWidth += g.fonts[fontName].MeasureString(s + ' ').X;
                        if (s.Contains("\n"))
                        {
                            //tempWidth = g.fonts[fontName].MeasureString(s.Substring(s.IndexOf("\n"), s.Length - 1)).X;
                            tempWidth = 0f;
                        }
                    }
                }


                ChangeText(tempText);
            }
        }

        protected override void Dispose()
        {
            base.Dispose();

            factory._gm._screen.RemoveText(this);
        }
    }
}
