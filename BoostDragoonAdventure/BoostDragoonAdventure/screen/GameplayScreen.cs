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
using Com.Brashmonkey.Spriter.player;


namespace wickedcrush.screen
{
    public class GameplayScreen : GameScreen
    {

        private GameplayManager gameplayManager;
        public Timer freezeFrameTimer = new Timer(150);
        Timer readyTimer, bgTimer;

        private bool testMode;

        RoomInfo _roomToTest; // needs to be expanded to to child class RoomTestScreen or something

        public Effect spriteEffect, unlitSpriteEffect, litSpriteEffect;

        public Texture2D background, bgDepth;
        public Queue<Texture2D> bgs = new Queue<Texture2D>();

        private List<SpriterPlayer> spriters = new List<SpriterPlayer>();
        private List<SpriterPlayer> addList = new List<SpriterPlayer>();
        private List<SpriterPlayer> removeList = new List<SpriterPlayer>();

        private List<TextEntity> screenText = new List<TextEntity>();

        float parallaxAmount = 1f, bgScale = 2f;
        Vector2 scroll = Vector2.Zero;
        Vector2 scrollIncrement = new Vector2(-1f, -1f);
        //Vector2 scrollIncrement = Vector2.Zero;
        
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

            bgTimer = new Timer(100);
            bgTimer.resetAndStart();

            //wow i'm stupid this will cause problems:
            gameplayManager.camera.cameraPosition = new Vector3(game.playerManager.getMeanPlayerPos().X - 320, game.playerManager.getMeanPlayerPos().Y - 240, 75f);// new Vector3(320f, 240f, 75f);

            
        }

        public void AddText(TextEntity text)
        {
            screenText.Add(text);
        }

        public void RemoveText(TextEntity text)
        {
            screenText.Remove(text);
        }

        public void AddSpriter(SpriterPlayer spriter)
        {
            if(!spriters.Contains(spriter) && !addList.Contains(spriter) && !removeList.Contains(spriter))
                addList.Add(spriter);
        }

        public void RemoveSpriter(SpriterPlayer spriter)
        {
            removeList.Add(spriter);
        }

        private void UpdateSpriters()
        {
            foreach (SpriterPlayer s in addList)
            {
                spriters.Add(s);
            }

            foreach (SpriterPlayer s in removeList)
            {
                spriters.Remove(s);
            }

            addList.Clear();
            removeList.Clear();


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
            spriteEffect = game.Content.Load<Effect>("fx/SpriteEffect");
            unlitSpriteEffect = game.Content.Load<Effect>("fx/UnlitSprite");
            litSpriteEffect = game.Content.Load<Effect>("fx/LitSprite");
            //background = game.Content.Load<Texture2D>(@"img/tex/water");
            bgs.Enqueue(game.Content.Load<Texture2D>(@"img/tex/stolen_water_1"));
            bgs.Enqueue(game.Content.Load<Texture2D>(@"img/tex/stolen_water_2"));
            bgs.Enqueue(game.Content.Load<Texture2D>(@"img/tex/stolen_water_3"));
            bgs.Enqueue(game.Content.Load<Texture2D>(@"img/tex/stolen_water_4"));
            bgs.Enqueue(game.Content.Load<Texture2D>(@"img/tex/stolen_water_5"));
            
            background = bgs.Dequeue();
            bgs.Enqueue(background);

            bgDepth = game.Content.Load<Texture2D>(@"img/tex/depth_test");


            //spriteEffect = new BasicEffect(game.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "";
            //game.diag += "Camera Position: " + gameplayManager.camera.cameraPosition.X + ", " + gameplayManager.camera.cameraPosition.Y + ", " + gameplayManager.camera.cameraPosition.Z + "\n";
            //game.diag += "fov: " + gameplayManager.camera.fov + "\n";
            //game.diag += gameTime.ElapsedGameTime.Milliseconds;
            //game.diag += "\n" + 
            UpdateSpriters();

            scroll += scrollIncrement;
            
            readyTimer.Update(gameTime);
            bgTimer.Update(gameTime);

            if (!readyTimer.isDone())
                return;

            if (bgTimer.isDone())
            {
                background = bgs.Dequeue();
                bgs.Enqueue(background);

                bgTimer.resetAndStart();
            }

            UpdateFreezeFrame(gameTime);

            if(!freezeFrameTimer.isActive() || freezeFrameTimer.isDone())
                gameplayManager.Update(gameTime);
            
            DebugControls();

        }

        public override void Render(RenderTarget2D renderTarget, RenderTarget2D depthTarget, RenderTarget2D spriteTarget)
        {
            //spriteEffect.Parameters["depth"].SetValue(0.5f);

            gameplayManager.scene.DrawScene(game, gameplayManager, renderTarget, depthTarget, spriteTarget, true);
            RenderSprites(renderTarget, depthTarget, spriteTarget, true, gameplayManager.scene.lightList);

            
            gameplayManager.scene.DrawScene(game, gameplayManager, renderTarget, depthTarget, spriteTarget, false);
            RenderSprites(renderTarget, depthTarget, spriteTarget, false, gameplayManager.scene.lightList);
        }

        public void RenderSprites(RenderTarget2D renderTarget, RenderTarget2D depthTarget, RenderTarget2D spriteTarget, bool depthPass, Dictionary<string, PointLightStruct> lightList)
        {
            
            if (!depthPass)
            {
                //game.GraphicsDevice.SetRenderTarget(renderTarget);

                game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null);

                game.spriteBatch.Draw(
                background,
                new Rectangle(0, 0, renderTarget.Width, renderTarget.Height),
                new Rectangle(
                    (int)(((float)renderTarget.Width / (1080f * gameplayManager.camera.fov)) * bgScale * gameplayManager.camera.cameraPosition.X * (2f / gameplayManager.camera.zoom) * 2.25f * parallaxAmount + scroll.X),
                    (int)(((float)renderTarget.Height / 1080f) * bgScale * gameplayManager.camera.cameraPosition.Y * (2f / gameplayManager.camera.zoom) * 2.25f * (float)(Math.Sqrt(2) / 2) * parallaxAmount + scroll.Y),
                    (int)(bgScale * renderTarget.Width), 
                    (int)(bgScale * renderTarget.Height)),
                Color.White, 0f,
                Vector2.Zero, SpriteEffects.None, 1f);
                game.spriteBatch.End();

                game.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, litSpriteEffect, game.spriteScale);
            }
            else
            {
                game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null);
                game.spriteBatch.Draw(bgDepth, new Rectangle(0, 0, depthTarget.Width, depthTarget.Height), Color.White);
                game.spriteBatch.End();

                game.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, spriteEffect, game.spriteScale);
                
            
            }
            
            

            //game.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, game.spriteScale);

            gameplayManager.entityManager.Draw(depthPass, lightList, gameplayManager);
            gameplayManager.particleManager.Draw(depthPass, lightList, gameplayManager);

            game.spriteBatch.End();

            //game.GraphicsDevice.SetRenderTarget(depthTarget);

            //game.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise, spriteEffect, game.spriteScale);

            //gameplayManager.entityManager.Draw(true);
            //gameplayManager.particleManager.Draw(true);

            //game.spriteBatch.End();
            
        }

        public override void Draw()
        {
            //game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, game.debugSpriteScale);
            //game.playerManager.DebugDrawPanels(game.spriteBatch, gameplayManager.camera, game.testFont);
            //game.spriteBatch.End();

            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, unlitSpriteEffect, game.spriteScale);
            game.playerManager.DrawHud();

            foreach (SpriterPlayer s in spriters)
            {
                game.spriterManager.DrawPlayer(s);
            }

            game.spriteBatch.End();

            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, game.spriteScale);
            
            foreach (TextEntity t in screenText)
            {
                t.HudDraw();
            }

            

            game.spriteBatch.End();
        }

        //public override void DebugDraw()
        //{
            
            //game.playerManager.DebugDrawPanels(game.spriteBatch, gameplayManager.camera, game.testFont);

            //DrawHud();

        //}

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
                if (game.instantAction)
                    game.Exit();

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

            gameplayManager.Dispose();

            game.screenManager.RemoveScreen(this);
        }
    }
}
