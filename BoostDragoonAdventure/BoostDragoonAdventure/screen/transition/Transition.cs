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
            UpdateTimers(gameTime);

            if (timers["transition"].isDone())
                Dispose();
        }

        public void Initialize(Game g, double transitionTime)
        {
            base.Initialize(g);

            timers = new Dictionary<String, Timer>();
            timers.Add("transition", new Timer(transitionTime));
            timers["transition"].resetAndStart();
        }

        protected float GetPercent()
        {
            return timers["transition"].getPercent();
        }

        public override void Dispose()
        {
            game.screenManager.RemoveScreen(this);
        }

        private void UpdateTimers(GameTime gameTime)
        {
            foreach (KeyValuePair<String, Timer> pair in timers)
            {
                pair.Value.Update(gameTime);
            }
        }
    }
}
