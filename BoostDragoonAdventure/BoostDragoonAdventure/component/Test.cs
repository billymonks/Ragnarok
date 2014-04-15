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
using wickedcrush.map;
using wickedcrush.controls;
using wickedcrush.entity.physics_entity.agent.player;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using wickedcrush.entity.physics_entity.agent;
using wickedcrush.player;
using wickedcrush.factory.entity;
using wickedcrush.stats;
using wickedcrush.manager.controls;
using wickedcrush.manager.player;
using wickedcrush.manager.entity;


namespace wickedcrush.component
{
    public class Test : Microsoft.Xna.Framework.GameComponent
    {
        public ControlsManager controlsManager;
        public PlayerManager playerManager;
        public EntityManager entityManager;

        public EntityFactory factory;
        
        Map testMap;
        SpriteFont testFont;

        Game game;

        World w;

        private Texture2D whiteTexture;

        public Test(Game game)
            : base(game)
        {
            w = new World(Vector2.Zero);
            w.Gravity = Vector2.Zero;

            controlsManager = new ControlsManager(game);
            playerManager = new PlayerManager(game, controlsManager);
            entityManager = new EntityManager(game);

            factory = new EntityFactory(entityManager, playerManager, controlsManager, w);

            LoadContent(game);

            this.game = game;
            
            Initialize();

            initializeWhiteTexture(game.GraphicsDevice);
        }

        public override void Initialize()
        {
            base.Initialize();

            

            testMap = new Map("I66", w);
            
            //factory = new EntityFactory(game.entityManager, game.playerManager, w);
            factory.setMap(testMap);
        }

        private void initializeWhiteTexture(GraphicsDevice gd)
        {
            whiteTexture = new Texture2D(gd, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.White;
            whiteTexture.SetData(data);
        }

        private void checkAndAdd()
        {
            controlsManager.checkAndAddGamepads();
        }

        

        private void LoadContent(Game game)
        {
            testFont = game.Content.Load<SpriteFont>("Fonts/TestFont");
        }

        public override void Update(GameTime gameTime)
        {
            w.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));

            game.diag = "";

            playerManager.Update(gameTime);
            entityManager.Update(gameTime);
            

            factory.Update(); //player creation, needs to be replaced

            controlsManager.Update(gameTime);

            DebugControls();

            base.Update(gameTime);
        }

        public void Draw(GraphicsDevice gd, SpriteBatch sb)
        {
            
            testMap.drawMap(gd, sb, testFont);
            DebugDraw(gd, sb);
            DrawPlayerHud(gd, sb);
            
        }

        private void DebugDraw(GraphicsDevice gd, SpriteBatch sb)
        {
            entityManager.DebugDraw(gd, sb, whiteTexture, testFont);

            playerManager.DebugDraw(gd, sb, whiteTexture, testFont);
        }

        private void DrawDiag(GraphicsDevice gd, SpriteBatch sb)
        {
            sb.DrawString(testFont, game.diag, new Vector2(2, 1), Color.Black);
            sb.DrawString(testFont, game.diag, new Vector2(2, 3), Color.Black);
            sb.DrawString(testFont, game.diag, new Vector2(3, 1), Color.Black);
            sb.DrawString(testFont, game.diag, new Vector2(3, 3), Color.Black);
            sb.DrawString(testFont, game.diag, new Vector2(4, 1), Color.Black);
            sb.DrawString(testFont, game.diag, new Vector2(4, 3), Color.Black);
            
            
            sb.DrawString(testFont, game.diag, new Vector2(3, 2), Color.White);
        }

        private void DrawPlayerHud(GraphicsDevice gd, SpriteBatch sb)
        {
            foreach (Player p in playerManager.getPlayerList())
            {
                sb.DrawString(testFont, p.name + "\nHP: " + p.getStats().hp + "/" + p.getStats().maxHP, new Vector2(p.playerNumber * 100 + 5, 5), Color.White);
            }
        }

        private void DebugControls()
        {
            if (controlsManager.debugControls.KeyPressed(Keys.P))
            {
                factory.addAgent(new Vector2(600, 130), new Vector2(24, 24), new Vector2(12, 12), true, new PersistedStats(5,5,5));
            }

            if (controlsManager.debugControls.KeyPressed(Keys.O))
            {
                factory.addAgent(new Vector2(600, 130), new Vector2(8, 8), new Vector2(4, 4), true, new PersistedStats(5, 5, 5));
            }
        }
    }
}
