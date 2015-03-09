using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.screen.transition;
using wickedcrush.task;
using System.IO;

namespace wickedcrush.manager.screen
{
    public class ScreenManager
    {
        private List<GameScreen> screenList = new List<GameScreen>();
        private List<GameScreen> screensToAdd = new List<GameScreen>();
        private List<GameScreen> screensToRemove = new List<GameScreen>();

        private LoadingScreen loadingScreen;

        private GameBase _game;

        BasicEffect basicEffect;

        RenderTarget2D renderTarget;
        RenderTarget2D renderTargetBlurred;
        RenderTarget2D renderTargetBlurred2;
        RenderTarget2D renderTargetDepth;

        Effect effectPostDoF;
        Effect gaussianBlurEffect;

        float focusDistance = 0;
        float focusRange = 0;
        float nearClip = 0;
        float farClip = 0;

        public ScreenManager(GameBase game, GameScreen rootScreen)
        {
            this._game = game;

            Initialize(rootScreen);
        }

        private void Initialize(GameScreen rootScreen)
        {
            renderTarget = new RenderTarget2D(
                _game.GraphicsDevice,
                _game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                _game.GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                _game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                _game.GraphicsDevice.PresentationParameters.DepthStencilFormat);

            renderTargetBlurred = new RenderTarget2D(
                _game.GraphicsDevice,
                _game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2,
                _game.GraphicsDevice.PresentationParameters.BackBufferHeight / 2,
                false,
                _game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            renderTargetBlurred2 = new RenderTarget2D(
                _game.GraphicsDevice,
                _game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2,
                _game.GraphicsDevice.PresentationParameters.BackBufferHeight / 2,
                false,
                _game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            renderTargetDepth = new RenderTarget2D(
                _game.GraphicsDevice,
                _game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                _game.GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                _game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                _game.GraphicsDevice.PresentationParameters.DepthStencilFormat);

            effectPostDoF = _game.Content.Load<Effect>("fx/PostProcessDoF");
            gaussianBlurEffect = _game.Content.Load<Effect>("fx/GaussianBlur");

            

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

            
            //_game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            _game.GraphicsDevice.SetRenderTarget(renderTarget);
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

                screenList[i].Render(renderTarget, renderTargetDepth);



                screenList[i].Draw();
                
                
            }

            //_game.GraphicsDevice.SetRenderTarget(renderTargetBlurred);

            //_game.GraphicsDevice.Clear(Color.Black);

            // Pass 2: draw from rendertarget 1 into rendertarget 2,
            // using a shader to apply a horizontal gaussian blur filter.
            SetBlurEffectParameters(1.0f / (float)renderTargetBlurred.Width, 0);

            DrawFullscreenQuad(renderTarget, renderTargetBlurred2,
                               gaussianBlurEffect,
                               IntermediateBuffer.BlurredHorizontally);

            // Pass 3: draw from rendertarget 2 back into rendertarget 1,
            // using a shader to apply a vertical gaussian blur filter.
            SetBlurEffectParameters(0, 1.0f / (float)renderTargetBlurred.Height);

            DrawFullscreenQuad(renderTargetBlurred2, renderTargetBlurred,
                               gaussianBlurEffect,
                               IntermediateBuffer.BlurredBothWays);

            // Pass 4: draw both rendertarget 1 and the original scene
            // image back into the main backbuffer, using a shader that
            // combines them to produce the final bloomed result.
            _game.GraphicsDevice.SetRenderTarget(null);

            //_game.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, gaussianBlurEffect);
            //_game.spriteBatch.Draw(renderTarget, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), Color.White);
            //_game.spriteBatch.End();

            //_game.GraphicsDevice.SetRenderTarget(null);

            if (_game.controlsManager.debugControls.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F2))
            {
                DateTime date = DateTime.Now; //Get the date for the file name
                Stream stream = File.Create("render" + date.ToString("MM-dd-yy H;mm;ss") + ".png");

                renderTarget.SaveAsPng(stream, renderTarget.Width, renderTarget.Height);
            }

            if (_game.controlsManager.debugControls.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F3))
            {
                DateTime date = DateTime.Now; //Get the date for the file name
                Stream stream = File.Create("blurred" + date.ToString("MM-dd-yy H;mm;ss") + ".png");

                renderTargetBlurred.SaveAsPng(stream, renderTarget.Width, renderTarget.Height);
            }

            SetShaderParameters(355, 215, 150, 850);

            _game.GraphicsDevice.BlendState = BlendState.Additive;
            _game.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, effectPostDoF);
            
            effectPostDoF.Parameters["D1M"].SetValue(renderTargetDepth);
            effectPostDoF.Parameters["BlurScene"].SetValue(renderTargetBlurred);
            _game.spriteBatch.Draw(renderTarget, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), Color.White);
            _game.spriteBatch.End();

            for (int i = screenIndex; i < screenList.Count; i++)
            {
                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, _game.debugSpriteScale);
                screenList[i].DebugDraw();
                _game.spriteBatch.End();

                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, _game.fullSpriteScale);

                screenList[i].FullScreenDraw();

                _game.spriteBatch.End();

                screenList[i].FreeDraw();
            }
        }

        void SetShaderParameters()
        {

        }

        void SetShaderParameters(float fD, float fR, float nC, float fC)
        {
            focusDistance = fD;
            focusRange = fR;
            nearClip = nC;
            farClip = fC;
            farClip = farClip / (farClip - nearClip);

            effectPostDoF.Parameters["Distance"].SetValue(focusDistance);
            effectPostDoF.Parameters["Range"].SetValue(focusRange);
            effectPostDoF.Parameters["Near"].SetValue(nearClip);
            effectPostDoF.Parameters["Far"].SetValue(farClip);
        }

        //MS bs -- needs heavy refactor

        // Optionally displays one of the intermediate buffers used
        // by the bloom postprocess, so you can see exactly what is
        // being drawn into each rendertarget.
        public enum IntermediateBuffer
        {
            PreBloom,
            BlurredHorizontally,
            BlurredBothWays,
            FinalResult,
        }

        public IntermediateBuffer ShowBuffer
        {
            get { return showBuffer; }
            set { showBuffer = value; }
        }

        IntermediateBuffer showBuffer = IntermediateBuffer.FinalResult;

        /// <summary>
        /// Helper for drawing a texture into a rendertarget, using
        /// a custom shader to apply postprocessing effects.
        /// </summary>
        void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget,
                                Effect effect, IntermediateBuffer currentBuffer)
        {
            _game.GraphicsDevice.SetRenderTarget(renderTarget);

            DrawFullscreenQuad(texture,
                               renderTarget.Width, renderTarget.Height,
                               effect, currentBuffer);
        }

        /// <summary>
        /// Helper for drawing a texture into the current rendertarget,
        /// using a custom shader to apply postprocessing effects.
        /// </summary>
        void DrawFullscreenQuad(Texture2D texture, int width, int height,
                                Effect effect, IntermediateBuffer currentBuffer)
        {
            // If the user has selected one of the show intermediate buffer options,
            // we still draw the quad to make sure the image will end up on the screen,
            // but might need to skip applying the custom pixel shader.
            if (showBuffer < currentBuffer)
            {
                effect = null;
            }

            _game.spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            _game.spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            _game.spriteBatch.End();
        }

        /// <summary>
        /// Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.
        /// </summary>
        void SetBlurEffectParameters(float dx, float dy)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = gaussianBlurEffect.Parameters["SampleWeights"];
            offsetsParameter = gaussianBlurEffect.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }

        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        float ComputeGaussian(float n)
        {
            float theta = 3.5f;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
    }
}
