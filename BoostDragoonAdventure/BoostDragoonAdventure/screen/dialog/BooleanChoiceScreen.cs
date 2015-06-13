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
    public class BooleanChoiceScreen : GameScreen
    {
        Player player;
        String text;
        Vector2 pos;

        bool choice = false;
        String key;
        int val;
        //String _key;
        EntityFactory _factory;

        //GameScreen yesScreen;
        //GameScreen noScreen;

        public BooleanChoiceScreen(GameBase g, Player player, String text, Vector2 pos, String key, int val, EntityFactory factory)
        {
            //_key = key;
            //this.yesScreen = yesScreen;
            //this.noScreen = noScreen;
            //nextScreen = this.noScreen;
            this.key = key;
            this.val = val;
            _factory = factory;
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

        public override void Dispose()
        {
            //if (nextScreen != null)
            //{
            game.screenManager.AddScreen(nextScreen);
            //}
            //_factory.savedBools[_key] = choice;
            game.screenManager.RemoveScreen(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (player.c.LeftPressed() || player.c.RightPressed())
            {
                choice = !choice;
            }

            if (player.c.InteractPressed())
            {
                //_choice = choice;
                if (choice)
                {
                    player.getStats().set(key, val);
                }
                Dispose();
            }
        }

        public override void Draw()
        {
            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, game.debugSpriteScale);
            DrawText();
            DrawYesNo();
            game.spriteBatch.End();
        }

        private void DrawText()
        {
            game.spriteBatch.DrawString(game.testFont, text, pos, Color.White);
            game.spriteBatch.DrawString(game.testFont, "E", pos + new Vector2(30f, 10f), Color.Blue);
        }

        private void DrawYesNo()
        {
            if (choice)
            {
                game.spriteBatch.DrawString(game.testFont, "Yes", pos + new Vector2(10f, 10f), Color.Yellow);
            }
            else
            {
                game.spriteBatch.DrawString(game.testFont, "No", pos + new Vector2(10f, 10f), Color.Yellow);
            }
        }
    }
}
