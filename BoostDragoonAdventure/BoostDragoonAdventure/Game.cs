using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
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
using wickedcrush.display.primitives;
using wickedcrush.inventory;
using wickedcrush.factory.menu.panel;
using wickedcrush.manager.map.room;
using wickedcrush.manager.audio;
using System.Collections.ObjectModel;
using wickedcrush.manager.network;

namespace wickedcrush
{
    public enum LayerType
    {
        WALL = 0,
        DEATHSOUP = 1,
        WIRING = 2,
        ENTITY = 3
    };

    public class Game : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SoundManager soundManager;
        public ControlsManager controlsManager;
        public PlayerManager playerManager;
        public MapManager mapManager;
        public NetworkManager networkManager;

        public SpriteBatch spriteBatch;

        public PanelFactory panelFactory;

        public Stack<GameScreen> screenStack;

        public Map testMap;
        public String mapName = "";

        public String diag = "";

        public Texture2D whiteTexture, arrowTexture;
        public SpriteFont testFont;

        public bool transitionFinished;

        private BasicEffect e;
        public float debugxscale, debugyscale, xscale, yscale, debugxtranslate, xtranslate;
        private Matrix debugSpriteScale, spriteScale, fullSpriteScale;

        public bool debugMode;

        public bool fullscreen = false;

        public Game()
        {
            debugMode = true;

            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;
            //SetFrameRate(120);
            graphics.ApplyChanges();

            if (fullscreen)
            {
                graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                graphics.IsFullScreen = true;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
                graphics.IsFullScreen = false;
            }

            
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();

            e = new BasicEffect(GraphicsDevice);



            debugxscale = (float)GraphicsDevice.Viewport.Width / 640f;
            debugyscale = (float)GraphicsDevice.Viewport.Height / 480f;
            debugxtranslate = (debugxscale - debugyscale) * 320f;
            debugSpriteScale = Matrix.CreateScale(debugyscale, debugyscale, 1)
                * Matrix.CreateTranslation(debugxtranslate, 0f, 0f);

            xscale = (float)GraphicsDevice.Viewport.Width / 1440f;
            yscale = (float)GraphicsDevice.Viewport.Height / 1080f;
            xtranslate = (xscale - yscale) * 720f;
            spriteScale = Matrix.CreateScale(yscale, yscale, 1)
                * Matrix.CreateTranslation(xtranslate, 0f, 0f);

            fullSpriteScale = Matrix.CreateScale(xscale, yscale, 1); //for effects that should be stretched across whole screen (possible examples: transitions, edge darkening, etc.)

            panelFactory = new PanelFactory();

            ItemServer.Initialize();

            screenStack = new Stack<GameScreen>();

            networkManager = new NetworkManager(this);

            soundManager = new SoundManager(Content);
            controlsManager = new ControlsManager(this);
            playerManager = new PlayerManager(this, controlsManager);
            mapManager = new MapManager(this);
            

            screenStack.Push(new PlayerSelect(this));

        }

        protected override void LoadContent()
        {
            //Content.RootDirectory = "Content";
            testFont = Content.Load<SpriteFont>("fonts/TestFont");
            arrowTexture = Content.Load<Texture2D>("debugcontent/img/arrow");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            initializeWhiteTexture(GraphicsDevice);

            PrimitiveDrawer.LoadContent(GraphicsDevice);
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


            soundManager.Update(gameTime);
            controlsManager.Update(gameTime);

            playerManager.Update(gameTime);

            networkManager.Update();

            screenStack.Peek().Update(gameTime);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightCyan);

            if (debugMode)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, debugSpriteScale);
                if (screenStack.Count > 0)
                    screenStack.Peek().DebugDraw();
                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, spriteScale);
                if (screenStack.Count > 0)
                    screenStack.Peek().Draw();
                spriteBatch.End();
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, fullSpriteScale);
            if (screenStack.Count > 0)
                screenStack.Peek().FullScreenDraw();
            spriteBatch.End();

            if (screenStack.Count > 0)
                screenStack.Peek().FreeDraw();

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
