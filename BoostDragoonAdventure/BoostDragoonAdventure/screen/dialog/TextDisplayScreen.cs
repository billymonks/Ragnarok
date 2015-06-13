using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.screen.dialog
{
    public class TextDisplayScreen : GameScreen
    {
        Player player;
        public String text;
        Vector2 pos;

        

        public TextDisplayScreen(GameBase g, Player player, String text, Vector2 pos)
        {
            Initialize(g, player, text, pos);
        }

        public TextDisplayScreen(GameBase g, Player player, String text, Vector2 pos, GameScreen nextScreen)
        {
            this.nextScreen = nextScreen;
            Initialize(g, player, text, pos);
        }

        public void Initialize(GameBase g, Player player, String text, Vector2 pos)
        {
            base.Initialize(g);

            this.player = player;
            this.text = text;
            this.pos = pos;

            exclusiveDraw = false;
            exclusiveUpdate = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (player.c.InteractPressed())
            {
                Dispose();
            }
        }

        public override void Draw()
        {
            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, game.debugSpriteScale);
            DrawText();
            game.spriteBatch.End();
        }

        private void DrawText()
        {
            game.spriteBatch.DrawString(game.testFont, text, pos, Color.White);
            game.spriteBatch.DrawString(game.testFont, "E", pos + new Vector2(10f, 10f), Color.Blue);
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
