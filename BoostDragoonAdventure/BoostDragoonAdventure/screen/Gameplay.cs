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
using wickedcrush.map.layer;
using wickedcrush.manager.audio;
using wickedcrush.display._3d;
using wickedcrush.manager.map.room;
using wickedcrush.menu.panel;


namespace wickedcrush.screen
{
    public class Gameplay : GameScreen
    {
        

        public Panel panel;

        private MapManager mm;

        

        public Gameplay(Game game)
        {
            mm = game.mapManager;

            LoadContent(game);

            this.game = game;
            
            Initialize(game);

            
        }

        public override void Initialize(Game g)
        {
            base.Initialize(g);

            mm.loadMap(mm.atlas[g.mapName]);
            //mm.loadMap(game.mapName, mm.w);

            //factory.setMap(game.testMap);

            mm.factory.spawnPlayers(0);
        }

        private void connectWiring(Map map)
        {
            Layer wiring = map.getLayer(LayerType.WIRING);

            mm.entityManager.connectWiring(wiring);
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

            mm.Update(gameTime);
            
            

            DebugControls();

            //game.testMap.connectTriggers();

            

        }

        public override void Draw()
        {

            //game.testMap.DebugDraw(game.GraphicsDevice, game.spriteBatch, game.testFont);
            
        }

        public override void DebugDraw()
        {
            mm.map.DebugDraw(game.whiteTexture, game.GraphicsDevice, game.spriteBatch, game.testFont, mm.camera);
            mm.entityManager.DebugDraw(game.GraphicsDevice, game.spriteBatch, game.whiteTexture, game.arrowTexture, game.testFont, mm.camera);
            game.playerManager.DebugDrawPanels(game.spriteBatch, mm.camera, game.testFont);

            DrawHud();

        }

        private void DrawHud()
        {
            
            game.playerManager.DrawPlayerHud(game.spriteBatch, game.testFont);
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
                mm.factory.addMurderer(new Vector2(600, 160), new Vector2(24, 24), new Vector2(12, 12), true, new PersistedStats(20,20));
                //soundManager.playSound("blast off");
            }

            if (game.controlsManager.debugControls.KeyPressed(Keys.O))
            {
                mm.factory.addMurderer(new Vector2(600, 160), new Vector2(12, 12), new Vector2(6f, 6f), true, new PersistedStats(5, 5));
            }

            if (game.controlsManager.debugControls.KeyPressed(Keys.Escape))
            {
                Dispose();
                game.screenStack.Pop();
            }
            
            /*foreach (Player p in game.playerManager.getPlayerList()) //move these foreach to playermanager, create methods that use all players
            {
                if (p.c.SelectPressed())
                {
                    Dispose();
                    game.screenStack.Pop();
                    return;
                }
            }*/
        }

        public override void Dispose()
        {
            //mm.entityManager.Dispose();
        }
    }
}
