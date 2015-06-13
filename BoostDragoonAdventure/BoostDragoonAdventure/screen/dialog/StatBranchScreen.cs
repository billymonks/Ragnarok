using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.player;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.utility;
using wickedcrush.factory.entity;

namespace wickedcrush.screen.dialog
{
    public class StatBranchScreen : GameScreen
    {
        Player player;
        String key;
        int val;
        public int branchInt = -1;

        public GameScreen yesScreen;
        public GameScreen noScreen;

        public StatBranchScreen(GameBase g, Player player, String key, int val, GameScreen yesScreen, GameScreen noScreen)
        {
            //_key = key;
            this.yesScreen = yesScreen;
            this.noScreen = noScreen;
            nextScreen = this.noScreen;
            Initialize(g, player, key, val);
        }

        public StatBranchScreen(GameBase g, Player player, String key, int val)
        {
            //_key = key;
            //nextScreen = this.noScreen;
            Initialize(g, player, key, val);
        }

        public void Initialize(GameBase g, Player player, String key, int val)
        {
            base.Initialize(g);


            this.player = player;
            this.key = key;
            this.val = val;
            //this.text = text;
            //this.pos = pos;

            exclusiveDraw = false;
            exclusiveUpdate = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (player.getStats().get(key).Equals(val) && yesScreen != null)
            {
                nextScreen = yesScreen;
            }
            else if (noScreen != null)
            {
                nextScreen = noScreen;
            }

            Dispose();
        }

        public override void Draw()
        {

        }

        public void AddToYes(GameScreen screen)
        {
            GameScreen temp;
            if (yesScreen != null)
            {
                temp = yesScreen;

                while (temp.nextScreen != null)
                    temp = temp.nextScreen;

                temp.nextScreen = screen;
            }
            else
            {
                yesScreen = screen;
            }
        }

        public void AddToNo(GameScreen screen)
        {
            GameScreen temp;
            if (noScreen != null)
            {
                temp = noScreen;

                while (temp.nextScreen != null)
                    temp = temp.nextScreen;

                temp.nextScreen = screen;
            }
            else
            {
                noScreen = screen;
            }
        }

        public override void Dispose()
        {
            if (nextScreen != null)
            {
                game.screenManager.AddScreen(nextScreen);
            }

            game.screenManager.RemoveScreen(this);
        }

        


    }
}
