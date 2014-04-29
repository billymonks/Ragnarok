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
using FarseerPhysics;


namespace wickedcrush.screen
{
    public class Gameplay : GameScreen
    {
        public EntityManager entityManager;

        public EntityFactory factory;
        
        

        World w;

        

        public Gameplay(Game game)
        {
            w = new World(Vector2.Zero);
            w.Gravity = Vector2.Zero;

            entityManager = new EntityManager(game);

            factory = new EntityFactory(entityManager, game.playerManager, game.controlsManager, w);

            LoadContent(game);

            this.game = game;
            
            Initialize(game);

            
        }

        public override void Initialize(Game g)
        {
            base.Initialize(g);

            

            game.testMap = new Map(game.mapName, w, factory);
            
            //factory = new EntityFactory(game.entityManager, game.playerManager, w);
            factory.setMap(game.testMap);

            factory.spawnPlayers();
        }

        

        private void checkAndAdd()
        {
            game.controlsManager.checkAndAddGamepads();
        }

        

        private void LoadContent(Game game)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            

            game.diag = "";

            //playerManager.Update(gameTime);
            entityManager.Update(gameTime);
            

            factory.Update(); //player creation, needs to be replaced

            DebugControls();

            

            //base.Update(gameTime);
            w.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
        }

        public override void Draw()
        {

            game.testMap.drawMap(game.GraphicsDevice, game.spriteBatch, game.testFont);
            DebugDraw();
            //DrawPlayerHud();
            
        }

        private void DebugDraw()
        {
            entityManager.DebugDraw(game.GraphicsDevice, game.spriteBatch, game.whiteTexture, game.testFont);
            game.playerManager.DrawPlayerHud(game.spriteBatch, game.testFont);
            //game.playerManager.DebugDraw(game.GraphicsDevice, game.spriteBatch, game.whiteTexture, game.testFont);
        }

        private void DrawDiag()
        {
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(2, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(2, 3), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 3), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(4, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(4, 3), Color.Black);


            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 2), Color.White);
        }

        

        private void DebugControls()
        {
            if (game.controlsManager.debugControls.KeyPressed(Keys.P))
            {
                factory.addAgent(new Vector2(600, 160), new Vector2(24, 24), new Vector2(12, 12), true, new PersistedStats(30,30,5));
            }

            if (game.controlsManager.debugControls.KeyPressed(Keys.O))
            {
                factory.addAgent(new Vector2(600, 160), new Vector2(12, 12), new Vector2(6f, 6f), true, new PersistedStats(5, 5, 5));
            }

            if (game.controlsManager.debugControls.KeyPressed(Keys.Escape))
            {
                Dispose();
                game.screenStack.Pop();
            }
            
            foreach (Player p in game.playerManager.getPlayerList()) //move these foreach to playermanager, create methods that use all players
            {
                if (p.c.SelectPressed())
                {
                    Dispose();
                    game.screenStack.Pop();
                    return;
                }
            }
        }

        public override void Dispose()
        {
            entityManager.Dispose();
        }
    }
}
