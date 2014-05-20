using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.utility;

namespace wickedcrush.screen.transition
{
    // Separate from gamestate stack. do not update when in effect, but DO render
    public abstract class Transition : GameScreen
    {
        public bool finished, playable;
        protected Dictionary<String, Timer> timers;

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(Game g)
        {
            base.Initialize(g);

            timers = new Dictionary<String, Timer>();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
