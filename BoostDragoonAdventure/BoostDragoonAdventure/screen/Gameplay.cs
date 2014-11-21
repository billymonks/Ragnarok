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
using wickedcrush.manager.gameplay.room;
using wickedcrush.menu.panel;
using wickedcrush.utility;
using wickedcrush.manager.gameplay;


namespace wickedcrush.screen
{
    public class Gameplay : GameScreen
    {

        private GameplayManager mm;
        public Timer freezeFrameTimer = new Timer(150);
        Timer readyTimer;
        
        public Gameplay(Game game, String mapName)
        {
            mm = game.gameplayManager;

            LoadContent(game);

            this.game = game;
            
            Initialize(game, mapName);

        }

        public void Initialize(Game g, String mapName)
        {
            base.Initialize(g);

            exclusiveDraw = true;
            exclusiveUpdate = true;

            mm.loadMap(mm.atlas[mapName]);
            
            mm.factory.spawnPlayers(0);

            readyTimer = new Timer(20);
            readyTimer.start();
        }

        public void UpdateFreezeFrame(GameTime gameTime)
        {
            if(mm.getFreezeFrame())
                freezeFrameTimer.resetAndStart();

            freezeFrameTimer.Update(gameTime);
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

            readyTimer.Update(gameTime);

            if (!readyTimer.isDone())
                return;

            UpdateFreezeFrame(gameTime);

            if(!freezeFrameTimer.isActive() || freezeFrameTimer.isDone())
                mm.Update(gameTime);
            
            DebugControls();

        }

        public override void Draw()
        {
            
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
            
            if (game.controlsManager.SelectPressed())
            {
                Dispose();
            }
            
        }

        public override void Dispose()
        {
            game.playerManager.saveAllPlayers();
            game.screenManager.RemoveScreen(this);
        }
    }
}
