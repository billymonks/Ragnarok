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
using wickedcrush.manager.audio;
using System.Collections.ObjectModel;
using wickedcrush.manager.network;
using wickedcrush.manager.gameplay;
using wickedcrush.manager.gameplay.room;
using wickedcrush.manager.screen;
using wickedcrush.manager.task;
using wickedcrush.manager.map;
using wickedcrush.display.spriter;
using wickedcrush.entity.physics_entity.agent.action;
using wickedcrush.utility.config;
using wickedcrush.display._3d.atlas;
using wickedcrush.manager.eventscript;
using System.IO;

namespace wickedcrush
{
    public enum LayerType
    {
        WALL = 0,
        DEATHSOUP = 1,
        WIRING = 2,
        ENTITY = 3,
        ART1 = 4,
        ART2 = 5
    };

    public class GameBase : Microsoft.Xna.Framework.Game
    {
        public TaskManager taskManager;
        public GraphicsDeviceManager graphics;
        public SoundManager soundManager;
        public ControlsManager controlsManager;
        public PlayerManager playerManager;
        public MapManager mapManager;
        public RoomManager roomManager;
        public NetworkManager networkManager;
        public ScreenManager screenManager;

        public SpriteBatch spriteBatch;

        public SpriterManager spriterManager;

        public PanelFactory panelFactory;

        public EventManager eventManager;
        

        public Map testMap;
        //public String mapName = "";

        public String diag = "";

        public Texture2D whiteTexture, arrowTexture, transparentBlackTexture, backgroundTexture;
        public SpriteFont testFont;
        public Dictionary<String, SpriteFont> fonts;

        public bool transitionFinished;

        private BasicEffect e;
        public float debugxscale, debugyscale, xscale, yscale, debugxtranslate, xtranslate;
        public Matrix debugSpriteScale, spriteScale, fullSpriteScale;

        public bool debugMode;

        public bool fullscreen = false;

        public float aspectRatio;

        public bool instantAction = false;

        public bool freezeFrame = false;

        public GameSettings settings = new GameSettings();

        public String path, characterPath;


        public GameBase()
        {
            debugMode = false;

            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            graphics.SynchronizeWithVerticalRetrace = true;
            
            IsFixedTimeStep = true;
            SetFrameRate(60);
            graphics.ApplyChanges();

            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\Wicked Crush");

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            characterPath = Path.Combine(path, "characters");

            if (!System.IO.Directory.Exists(characterPath))
            {
                System.IO.Directory.CreateDirectory(characterPath);
            }



            spriterManager = new SpriterManager(this);
            playerManager = new PlayerManager(this);

            //Initialize();
        }

        protected void CalculateDimensionScale()
        {
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
        }

        protected override void Initialize()
        {
            base.Initialize();

            e = new BasicEffect(GraphicsDevice);

            CalculateDimensionScale();

            panelFactory = new PanelFactory(spriterManager);

            InventoryServer.Initialize();
            SkillServer.Initialize();

            //spriterManager = new SpriterManager(this);

            eventManager = new EventManager(this);

            networkManager = new NetworkManager(this);

            taskManager = new TaskManager(this);
            soundManager = new SoundManager(Content);
            controlsManager = new ControlsManager(this);
            
            mapManager = new MapManager(this);
            roomManager = new RoomManager();
            playerManager.Initialize();

        }

        protected override void LoadContent()
        {
            //Content.RootDirectory = "Content";
            testFont = Content.Load<SpriteFont>("fonts/TestFont");
            fonts = new Dictionary<string, SpriteFont>();
            fonts.Add("Khula", Content.Load<SpriteFont>("fonts/Khula"));
            fonts.Add("Rubik Mono One", Content.Load<SpriteFont>("fonts/Rubik Mono One"));
            fonts.Add("Bonbon", Content.Load<SpriteFont>("fonts/Bonbon"));
            arrowTexture = Content.Load<Texture2D>("debugcontent/img/arrow");
            backgroundTexture = Content.Load<Texture2D>("img/tex/big/largeshopwindow2");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            initializeWhiteTexture(GraphicsDevice);

            PrimitiveDrawer.LoadContent(GraphicsDevice);

            spriterManager.LoadContent();
            playerManager.LoadContent();

            
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            spriteBatch.Dispose();
            spriterManager.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            taskManager.Update(gameTime);
            
            soundManager.Update(gameTime);
            if (!freezeFrame)
            {
                controlsManager.Update(gameTime);
            }

            networkManager.Update(gameTime);

            screenManager.Update(gameTime);

            base.Update(gameTime);

            double fps = (1000 / gameTime.ElapsedGameTime.TotalMilliseconds);
            fps = Math.Round(fps, 0);
            Window.Title = "Wicked Crush Adventure";
        }

        

        

        protected override void Draw(GameTime gameTime)
        {
            screenManager.Draw(gameTime);

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
