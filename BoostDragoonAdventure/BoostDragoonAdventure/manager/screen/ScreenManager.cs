using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.screen.transition;
using wickedcrush.task;

namespace wickedcrush.manager.screen
{
    public class ScreenManager
    {
        private List<GameScreen> screenList = new List<GameScreen>();
        private List<GameScreen> screensToAdd = new List<GameScreen>();
        private List<GameScreen> screensToRemove = new List<GameScreen>();

        private LoadingScreen loadingScreen;

        private Game _game;

        BasicEffect basicEffect;

        public ScreenManager(Game game, GameScreen rootScreen)
        {
            this._game = game;

            Initialize(rootScreen);
        }

        private void Initialize(GameScreen rootScreen)
        {
            basicEffect = new BasicEffect(_game.GraphicsDevice);
            screenList.Add(rootScreen);
            loadingScreen = new LoadingScreen(_game);
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

            RemoveScreens();
            AddScreens();
        }

        public void StartLoading()
        {
            //AddScreen(new SolidColorFadeTransition(_game, 1000, true, new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f)));
            AddScreen(loadingScreen);
        }

        public void EndLoading()
        {
            RemoveScreen(loadingScreen);
            if (screensToAdd.Contains(loadingScreen))
                screensToAdd.Remove(loadingScreen);
        }

        public void AddScreen(GameScreen screen) //moves screen to top if already exists
        {
            screensToAdd.Add(screen);
        }

        public void AddScreen(GameScreen screen, bool autoDispose) 
        {
            screensToAdd.Add(screen);
            if (autoDispose)
            {
                _game.taskManager.EnqueueTask(
                new GameTask(
                    g => screen.finished,
                    g =>
                    {
                        screen.Dispose();
                    }
                ));
            }
        }

        public void RemoveScreen(GameScreen screen)
        {
            screensToRemove.Add(screen);
        }

        private void AddScreens()
        {
            foreach (GameScreen screen in screensToAdd)
            {
                if (screenList.Contains(screen))
                    screenList.Remove(screen);
                screenList.Add(screen);
            }

            screensToAdd.Clear();
        }

        private void RemoveScreens()
        {
            foreach (GameScreen screen in screensToRemove)
            {
                if(screenList.Contains(screen))
                    screenList.Remove(screen);
            }

            screensToRemove.Clear();
        }

        public void Draw(GameTime gameTime)
        {
            
            //basicEffect.
            _game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            
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

                //if (_game.debugMode)
                //{
                    
                //}
                //else
                //{
                screenList[i].Render();

                _game.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, _game.spriteScale);
                screenList[i].Draw();
                _game.spriteBatch.End();

                    _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, _game.debugSpriteScale);
                    screenList[i].DebugDraw();
                    _game.spriteBatch.End();
                //}

                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, _game.fullSpriteScale);

                screenList[i].FullScreenDraw();

                _game.spriteBatch.End();

                screenList[i].FreeDraw();
            }
        }
    }
}
