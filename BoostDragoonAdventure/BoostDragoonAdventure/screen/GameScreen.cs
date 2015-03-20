using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.screen
{
    public abstract class GameScreen
    {
        public GameBase game { get; set; }

        public bool exclusiveUpdate = false;
        public bool exclusiveDraw = false;
        public bool disposed = false;
        public bool finished = false;

        public event EventHandler<EventArgs> Disposed;

        public abstract void Dispose();

        public virtual void Initialize(GameBase g)
        {
            game = g;
        }
        public abstract void Update(GameTime gameTime);
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
    }
}
