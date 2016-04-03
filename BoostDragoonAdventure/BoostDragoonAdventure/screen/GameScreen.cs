using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.entity;
using Com.Brashmonkey.Spriter.player;

namespace wickedcrush.screen
{
    public abstract class GameScreen
    {
        public GameBase game { get; set; }

        public bool exclusiveUpdate = false;
        public bool exclusiveDraw = false;
        public bool disposed = false;
        public bool finished = false;

        protected List<SpriterPlayer> spriters = new List<SpriterPlayer>();
        protected List<SpriterPlayer> addList = new List<SpriterPlayer>();
        protected List<SpriterPlayer> removeList = new List<SpriterPlayer>();

        protected List<TextEntity> screenText = new List<TextEntity>();

        public GameScreen nextScreen;

        public event EventHandler<EventArgs> Disposed;

        public abstract void Dispose();

        public virtual void Initialize(GameBase g)
        {
            game = g;
        }
        public virtual void Update(GameTime gameTime)
        {
            UpdateSpriters();
            UpdateText(gameTime);
        }
        public virtual void Render(RenderTarget2D renderTarget, RenderTarget2D depthTarget, RenderTarget2D spriteTarget) { }
        public virtual void Draw() { }
        public virtual void DebugDraw() { }
        public virtual void FullScreenDraw() { }
        public virtual void FreeDraw() { }

        protected void DrawDiag()
        {

            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(2, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(2, 3), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 3), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(4, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(4, 3), Color.Black);


            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 2), Color.White);

        }

        public void AddText(TextEntity text)
        {
            screenText.Add(text);
        }

        public void RemoveText(TextEntity text)
        {
            screenText.Remove(text);
        }

        public void ClearText()
        {
            screenText.Clear();
        }

        public void AddSpriter(SpriterPlayer spriter)
        {
            if (!spriters.Contains(spriter) && !addList.Contains(spriter) && !removeList.Contains(spriter))
                addList.Add(spriter);
        }

        public void RemoveSpriter(SpriterPlayer spriter)
        {
            removeList.Add(spriter);
        }

        protected void UpdateSpriters()
        {
            foreach (SpriterPlayer s in addList)
            {
                spriters.Add(s);
            }

            foreach (SpriterPlayer s in removeList)
            {
                spriters.Remove(s);
            }

            addList.Clear();
            removeList.Clear();


        }

        protected void UpdateText(GameTime gameTime)
        {
            for(int i = screenText.Count-1; i>=0; i--)
            //foreach (TextEntity t in screenText)
            {
                screenText[i].Update(gameTime);
            }
        }
    }
}
