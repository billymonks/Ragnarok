using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.display.sprite;
using Microsoft.Xna.Framework;

namespace wickedcrush.manager.sprite
{
    public class SpriteManager : Microsoft.Xna.Framework.GameComponent
    {
        private List<BaseSprite> spriteList = new List<BaseSprite>();
        private List<BaseSprite> addList = new List<BaseSprite>();
        private List<BaseSprite> removeList = new List<BaseSprite>();

        public SpriteManager(GameBase game)
            : base(game)
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            updateSprites(gameTime);

            base.Update(gameTime);
        }

        private void updateSprites(GameTime gameTime)
        {
            foreach (BaseSprite s in spriteList)
            {
                s.Update(gameTime);

                if (s.readyForRemoval())
                    removeList.Add(s);
            }

            performRemoval();
            performAdd();
        }

        private void performAdd()
        {
            if (addList.Count > 0)
            {
                foreach (BaseSprite s in addList)
                {
                    spriteList.Add(s);
                }

                addList.Clear();
            }
        }

        private void performRemoval()
        {
            if (removeList.Count > 0)
            {
                foreach (BaseSprite s in removeList)
                {
                    spriteList.Remove(s);

                }

                removeList.Clear();
            }
        }

        public void AddSprite(BaseSprite s)
        {
            addList.Add(s);
        }
    }
}
