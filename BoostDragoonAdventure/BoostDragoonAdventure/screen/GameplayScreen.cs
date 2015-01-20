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
using wickedcrush.editor;


namespace wickedcrush.screen
{
    public class GameplayScreen : GameScreen
    {

        private GameplayManager gameplayManager;
        public Timer freezeFrameTimer = new Timer(150);
        Timer readyTimer;

        private bool testMode;

        RoomInfo _roomToTest; // needs to be expanded to to child class RoomTestScreen or something
        
        public GameplayScreen(GameBase game, String mapName)
        {
            gameplayManager = new GameplayManager(game, this, false);

            LoadContent(game);

            this.game = game;
            
            Initialize(game, mapName, false);

        }

        public GameplayScreen(GameBase game, RoomInfo roomToTest) // needs to be expanded to to child class RoomTestScreen or something
        {
            gameplayManager = new GameplayManager(game, this, true);

            LoadContent(game);
            this.game = game;
            _roomToTest = roomToTest;

            Initialize(game, "testMap", true);
        }

        public void Initialize(GameBase g, String mapName, bool testMode)
        {
            base.Initialize(g);

            exclusiveDraw = true;
            exclusiveUpdate = true;

            this.testMode = testMode;

            if (this.testMode)
            {
                g.playerManager.SaveTempStats();
            }

            gameplayManager.LoadMap(mapName);
            
            
            gameplayManager.factory.spawnPlayers(0);

            readyTimer = new Timer(20);
            readyTimer.start();

            
        }

        public RoomInfo GetRoom()
        {
            return _roomToTest;
        }

        public void UpdateFreezeFrame(GameTime gameTime)
        {
            if(gameplayManager.getFreezeFrame())
                freezeFrameTimer.resetAndStart();

            freezeFrameTimer.Update(gameTime);
        }

        private void connectWiring(Map map)
        {
            Layer wiring = map.getLayer(LayerType.WIRING);

            gameplayManager.entityManager.connectWiring(wiring);
        }

        private void checkAndAdd()
        {
            game.controlsManager.checkAndAddGamepads();
        }

        public void SetRoomReady()
        {
            if (_roomToTest != null)
            {
                _roomToTest.readyToAuthor = true;
                //_roomToTest.saveRoom();
                //game.roomManager.AddRoomToLocalAtlas(_roomToTest.stats);
            }
        }

        private void LoadContent(GameBase game)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "";
            game.diag += "Camera Position: " + gameplayManager.camera.cameraPosition.X + ", " + gameplayManager.camera.cameraPosition.Y;

            
            readyTimer.Update(gameTime);

            if (!readyTimer.isDone())
                return;

            UpdateFreezeFrame(gameTime);

            if(!freezeFrameTimer.isActive() || freezeFrameTimer.isDone())
                gameplayManager.Update(gameTime);
            
            DebugControls();

        }

        public override void Render()
        {
            gameplayManager.scene.DrawScene(game, gameplayManager);
        }

        public override void Draw()
        {
            //game.GraphicsDevice.Clear(Color.Black);
            game.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, game.spriteScale);
            gameplayManager.entityManager.Draw();
            game.spriteBatch.End();
            
            
        }

        public override void DebugDraw()
        {
            
            //gameplayManager.map.DebugDraw(game.whiteTexture, game.GraphicsDevice, game.spriteBatch, game.testFont, gameplayManager.camera);
            //gameplayManager.entityManager.DebugDraw(game.GraphicsDevice, game.spriteBatch, game.whiteTexture, game.arrowTexture, game.testFont, gameplayManager.camera);
            game.playerManager.DebugDrawPanels(game.spriteBatch, gameplayManager.camera, game.testFont);

            DrawHud();

        }

        public override void FreeDraw()
        {
            
            gameplayManager.entityManager.FreeDraw();

            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, Matrix.Identity);
            DrawDiag();
            game.spriteBatch.End();
        }

        private void DrawHud()
        {
            
            game.playerManager.DrawPlayerHud(game.spriteBatch, game.testFont);
        }

        /*private void DrawDiag()
        {
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(2, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(2, 3), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 3), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(4, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(4, 3), Color.Black);

            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 2), Color.White);
        }*/

        

        private void DebugControls()
        {
            
            if (game.controlsManager.SelectPressed())
            {
                Dispose();
            }
            
        }

        public override void Dispose()
        {
            if (testMode)
            {
                game.playerManager.LoadTempStats();
            } else {
                game.playerManager.saveAllPlayers();
            }
            game.screenManager.RemoveScreen(this);
        }
    }
}
