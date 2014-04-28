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
using wickedcrush.screen;
using wickedcrush.manager.controls;
using wickedcrush.manager.player;
using wickedcrush.manager.entity;
using wickedcrush.factory.entity;
using wickedcrush.map;

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
        public PlayerManager playerManager;

        public SpriteBatch spriteBatch;

        BasicEffect e;
        float xscale, yscale;
        Matrix spriteScale;

        public Stack<GameScreen> screenStack;

        public Map testMap;
        public String mapName = "";

        //Test test;

        public String diag = "";

        public Texture2D whiteTexture;
        public SpriteFont testFont;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";

            graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;
            //SetFrameRate(120);
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();

            e = new BasicEffect(GraphicsDevice);

            xscale = (float)GraphicsDevice.Viewport.Width / 640f;
            yscale = (float)GraphicsDevice.Viewport.Height / 480f;
            spriteScale = Matrix.CreateScale(yscale, yscale, 1);

            screenStack = new Stack<GameScreen>();

            controlsManager = new ControlsManager(this);
            playerManager = new PlayerManager(this, controlsManager);


            //componentStack.Push(new Test(this));
            screenStack.Push(new MapSelector(this));
            
        }

        protected override void LoadContent()
        {
            testFont = Content.Load<SpriteFont>("Fonts/TestFont");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            initializeWhiteTexture(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            spriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            if (screenStack.Count == 0)
            {
                this.Exit();
                return;
            }

            
            playerManager.Update(gameTime);
            
            screenStack.Peek().Update(gameTime);

            controlsManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightCyan);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, RasterizerState.CullNone, null, spriteScale);
            if(screenStack.Count>0)
                screenStack.Peek().Draw();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SetFrameRate(float frames)
        {
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / frames);
            graphics.ApplyChanges();
        }

        private void initializeWhiteTexture(GraphicsDevice gd)
        {
            whiteTexture = new Texture2D(gd, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.White;
            whiteTexture.SetData(data);
        }
    }
}
