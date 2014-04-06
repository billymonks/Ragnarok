using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using wickedcrush.entity;
using wickedcrush.component;
using wickedcrush.manager.controls;

namespace wickedcrush
{
    public enum LayerType
    {
        WALL = 0,
        DEATH_SOUP = 1
    };

    public class Game : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public ControlsManager controlsManager;

        SpriteBatch spriteBatch;

        BasicEffect e;
        float xscale, yscale;
        Matrix spriteScale;

        Test test;

        public String diag = "";

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            controlsManager = new ControlsManager(this);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";

            graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;
            graphics.ApplyChanges();
            
        }

        protected override void Initialize()
        {
            base.Initialize();

            SetFrameRate(60); //internal frame rate

            e = new BasicEffect(GraphicsDevice);

            xscale = (float)GraphicsDevice.Viewport.Width / 640f;
            yscale = (float)GraphicsDevice.Viewport.Height / 480f;
            spriteScale = Matrix.CreateScale(yscale, yscale, 1);

            test = new Test(this);
            
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            spriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            test.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightCyan);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, RasterizerState.CullNone, null, spriteScale);
            test.Draw(GraphicsDevice, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SetFrameRate(float frames)
        {
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / frames);
            graphics.ApplyChanges();
        }
    }
}
