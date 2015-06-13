using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.screen.dialog
{
    //wtf??!?!?!?
    public class StatSetScreen : GameScreen
    {
        Player player;
        String key;
        int val;

        

        public StatSetScreen(GameBase g, Player player, String key, int val, GameScreen nextScreen)
        {
            this.nextScreen = nextScreen;
            Initialize(g, player, key, val);
        }

        public StatSetScreen(GameBase g, Player player, String key, int val)
        {
            Initialize(g, player, key, val);
        }

        public void Initialize(GameBase g, Player player, String key, int val)
        {
            base.Initialize(g);

            this.player = player;
            this.key = key;
            this.val = val;
            //player.getStats().set(key, val);

            //exclusiveDraw = false;
            //exclusiveUpdate = true;

            Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            player.getStats().set(key, val);
            Dispose();
            //if (player.c.InteractPressed())
            //{
                //Dispose();
           // }
        }

        public override void Draw()
        {
            //game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, game.debugSpriteScale);
            //DrawText();
            //game.spriteBatch.End();
        }

        private void DrawText()
        {
            //game.spriteBatch.DrawString(game.testFont, text, pos, Color.White);
            //game.spriteBatch.DrawString(game.testFont, "E", pos + new Vector2(10f, 10f), Color.Blue);
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
