using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.display.animation;
using Microsoft.Xna.Framework;

namespace wickedcrush.manager.tween
{

    // a mistake, tween should be a sprite method
    public class TweenManager : Microsoft.Xna.Framework.GameComponent
    {
        GameBase g;

        private List<Tween> tweenList = new List<Tween>();
        private List<Tween> addList = new List<Tween>();
        private List<Tween> removeList = new List<Tween>();

        public TweenManager(GameBase game)
            : base(game)
        {
            g = game;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateTweens(gameTime);

            base.Update(gameTime);
        }

        private void UpdateTweens(GameTime gameTime)
        {
            foreach (Tween t in tweenList)
            {
                t.Update(gameTime);

                if (t.readyForRemoval)
                {
                    removeList.Add(t);
                }
            }

            performRemoval();
            performAdd();
        }

        private void performAdd()
        {
            if (addList.Count > 0)
            {
                foreach (Tween t in addList)
                {
                    tweenList.Add(t);
                }

                addList.Clear();
            }
        }

        private void performRemoval()
        {
            if (removeList.Count > 0)
            {
                foreach (Tween t in removeList)
                {
                    tweenList.Remove(t);

                }

                removeList.Clear();
            }
        }
    }
}
