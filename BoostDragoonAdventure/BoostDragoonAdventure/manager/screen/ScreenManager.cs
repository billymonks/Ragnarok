using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.manager.screen
{
    public class ScreenManager
    {
        private List<GameScreen> screenList = new List<GameScreen>(); //need GameScreen manager
        private List<GameScreen> screensToAdd = new List<GameScreen>();
        private List<GameScreen> screensToRemove = new List<GameScreen>();

        private Game _game;

        public ScreenManager(Game game, GameScreen rootScreen)
        {
            this._game = game;

            Initialize(rootScreen);
        }

        private void Initialize(GameScreen rootScreen)
        {
            screenList.Add(rootScreen);
        }

        public void Update(GameTime gameTime)
        {
            if (screenList.Count == 0)
            {
                _game.networkManager.Disconnect();
                _game.Exit();
                return;
            }

            for (int i = screenList.Count - 1; i >= 0; i--)
            {
                screenList[i].Update(gameTime);
                if (screenList[i].exclusiveUpdate)
                    break;
            }

            AddScreens();
            RemoveScreens();
        }

        public void AddScreen(GameScreen screen)
        {
            screensToAdd.Add(screen);
        }

        public void RemoveScreen(GameScreen screen)
        {
            screensToRemove.Add(screen);
        }

        private void AddScreens()
        {
            foreach (GameScreen screen in screensToAdd)
            {
                screenList.Add(screen);
            }

            screensToAdd.Clear();
        }

        private void RemoveScreens()
        {
            foreach (GameScreen screen in screensToRemove)
            {
                screenList.Remove(screen);
            }

            screensToRemove.Clear();
        }

        public void Draw(GameTime gameTime)
        {
            _game.GraphicsDevice.Clear(Color.LightCyan);

            int screenIndex = 0;

            for (int i = screenList.Count - 1; i >= 0; i--)
            {
                if (screenList[i].exclusiveDraw)
                {
                    screenIndex = i;
                    break;
                }
            }

            for (int i = screenIndex; i < screenList.Count; i++)
            {

                if (_game.debugMode)
                {
                    _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, _game.debugSpriteScale);
                    screenList[i].DebugDraw();
                    _game.spriteBatch.End();
                }
                else
                {
                    _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, _game.spriteScale);
                    screenList[i].Draw();
                    _game.spriteBatch.End();
                }

                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, _game.fullSpriteScale);

                screenList[i].FullScreenDraw();

                _game.spriteBatch.End();

                screenList[i].FreeDraw();
            }
        }
    }
}
