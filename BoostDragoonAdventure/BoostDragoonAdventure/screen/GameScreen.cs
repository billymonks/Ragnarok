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
        public Game game { get; set; }

        public event EventHandler<EventArgs> Disposed;

        public abstract void Dispose();
        //protected abstract void Dispose(bool disposing);
        public virtual void Initialize(Game g)
        {
            game = g;
        }
        public abstract void Update(GameTime gameTime);
        public virtual void Draw() { }
        public virtual void DebugDraw() { }
        public virtual void FullScreenDraw() { }
        public virtual void FreeDraw() { }
    }
}
