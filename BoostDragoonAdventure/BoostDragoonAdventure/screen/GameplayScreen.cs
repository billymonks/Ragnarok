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
        public Timer freezeFrameTimer = new Timer(100);
        Timer readyTimer, bgTimer;

        private bool testMode;

        RoomInfo _roomToTest; // needs to be expanded to to child class RoomTestScreen or something

        public Effect spriteEffect, litSpriteEffect;

        public Texture2D background, bgDepth;
        public Queue<Texture2D> bgs = new Queue<Texture2D>();

        

        float parallaxAmount = 1f, bgScale = 2f;
        Vector2 scroll = Vector2.Zero;
        Vector2 scrollIncrement = new Vector2(-1f, -1f);

        public TextEntity currencyText, hpText, epText;
        //Vector2 scrollIncrement = Vector2.Zero;
        
        public GameplayScreen(GameBase game, String mapName)
        {
            gameplayManager = new GameplayManager(game, this, false);

            LoadContent(game);

            this.game = game;
            
            Initialize(game, mapName, false);

            game.screenManager.EndLoading();

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
            
            
            gameplayManager.factory.spawnPlayers();

            readyTimer = new Timer(20);
            readyTimer.start();

            bgTimer = new Timer(100);
            bgTimer.resetAndStart();

            //wow i'm stupid this will cause problems:
            gameplayManager.camera.cameraPosition = new Vector3(game.playerManager.getMeanPlayerPos().X - 320, game.playerManager.getMeanPlayerPos().Y - 240, 75f);// new Vector3(320f, 240f, 75f);

            currencyText = new TextEntity("aaaaaaa", new Vector2(1400, 1080), g.soundManager, g, -1, gameplayManager.factory, 1f, false);
            currencyText.alignment = TextAlignment.Right;
            currencyText.SetSpeed(0);

            hpText = new TextEntity("hp", new Vector2(125, 990), g.soundManager, g, -1, gameplayManager.factory, 0.75f, false);
            hpText.textColor = Color.Pink;
            hpText.alignment = TextAlignment.Left;
            hpText.SetSpeed(0);

            epText = new TextEntity("ep", new Vector2(145, 1020), g.soundManager, g, -1, gameplayManager.factory, 0.75f, false);
            epText.textColor = Color.MintCream;
            epText.alignment = TextAlignment.Left;
            epText.SetSpeed(0);

            AddText(currencyText);
            AddText(hpText);
            AddText(epText);
            
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
            base.Update(gameTime);
            game.diag = "";
            //game.diag += "Camera Position: " + gameplayManager.camera.cameraPosition.X + ", " + gameplayManager.camera.cameraPosition.Y + ", " + gameplayManager.camera.cameraPosition.Z + "\n";
            //game.diag += "fov: " + gameplayManager.camera.fov + "\n";
            //game.diag += gameTime.ElapsedGameTime.Milliseconds;
            //game.diag += "\n" + 
            //UpdateSpriters();

            currencyText.ChangeText(gameplayManager._playerManager.getPlayerList()[0].getStats().inventory.currency.ToString());

            hpText.ChangeText(gameplayManager._playerManager.getPlayerList()[0].getStats().getNumber("hp").ToString());
            epText.ChangeText(gameplayManager._playerManager.getPlayerList()[0].getStats().getNumber("boost").ToString());

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

            //foreach (TextEntity t in screenText)
            //{
                //t.Update(gameTime);
            //}

            UpdateFreezeFrame(gameTime);

            if (!freezeFrameTimer.isActive() || freezeFrameTimer.isDone())
            {
                gameplayManager.Update(gameTime, false);
                this.game.freezeFrame = false;
            }
            else
            {
                gameplayManager.Update(gameTime, true);
                this.game.freezeFrame = true;
            }
            //DebugControls();

        }

        public override void Render(RenderTarget2D renderTarget, RenderTarget2D depthTarget, RenderTarget2D spriteTarget)
        {
            //spriteEffect.Parameters["depth"].SetValue(0.5f);

            gameplayManager.scene.DrawScene(game, gameplayManager, renderTarget, depthTarget, spriteTarget, true);
            RenderSprites(renderTarget, depthTarget, spriteTarget, true, gameplayManager.scene.lightDictionary);

            
            gameplayManager.scene.DrawScene(game, gameplayManager, renderTarget, depthTarget, spriteTarget, false);
            RenderSprites(renderTarget, depthTarget, spriteTarget, false, gameplayManager.scene.lightDictionary);
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

            gameplayManager.entityManager.Draw(depthPass, lightList, gameplayManager, gameplayManager.camera);
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

            game.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, game.spriterManager.unlitSpriteEffect, game.spriteScale);
            game.playerManager.DrawHud();

            foreach (SpriterPlayer s in spriters)
            {
                game.spriterManager.DrawPlayer(s);
            }

            game.spriteBatch.End();

            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, game.spriteScale);
            
            foreach (TextEntity t in screenText)
            {
                t.HudDraw(t.inScene, true);
            }

            

            game.spriteBatch.End();
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
