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


namespace wickedcrush.component
{
    public class Test : Microsoft.Xna.Framework.GameComponent
    {
        List<Entity> playerList;
        Map testMap;
        SpriteFont testFont;

        Game game;

        World w;

        private Texture2D whiteTexture;

        public Test(Game game)
            : base(game)
        {
            LoadContent(game);

            this.game = game;
            
            Initialize();

            initializeWhiteTexture(game.GraphicsDevice);
        }

        public override void Initialize()
        {
            base.Initialize();

            w = new World(Vector2.Zero);
            w.Gravity = Vector2.Zero;

            testMap = new Map(640, 480, "Test Map");
            bool[,] testData = new bool[3,3];
            //testData[0, 0] = true;
            testData[1 , 1] = true;
            testData[2, 2] = true;
            testMap.addLayer(testData, LayerType.WALL);

            playerList = new List<Entity>();
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
            game.controlsManager.checkAndAddGamepads();
        }

        private void addPlayer(Controls controls)
        {
            playerList.Add(new PlayerAgent(w, new Vector2(100, 100), new Vector2(60, 60), new Vector2(30, 30), 1f, controls));
        }

        private void LoadContent(Game game)
        {
            testFont = game.Content.Load<SpriteFont>("Fonts/TestFont");
        }

        public override void Update(GameTime gameTime)
        {
            w.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));

            game.diag = "";

            game.controlsManager.Update(gameTime);
            UpdatePlayers(gameTime);

            if (game.controlsManager.gamepadPressed())
            {
                addPlayer(game.controlsManager.checkAndAddGamepads());
            }

            if (game.controlsManager.keyboardPressed())
            {
                addPlayer(game.controlsManager.addKeyboard());
            }

            game.diag += game.controlsManager.getDiag();
            base.Update(gameTime);
        }

        public void Draw(GraphicsDevice gd, SpriteBatch sb)
        {
            
            testMap.drawMap(gd, sb, testFont);
            DebugDraw(gd, sb);
            DrawDiag(gd, sb);
            
        }

        private void UpdatePlayers(GameTime gameTime)
        {
            List<Entity> newTempList = new List<Entity>(playerList);
            foreach (PlayerAgent p in playerList)
            {

                newTempList.Remove(p);
                p.Update(gameTime);
            }
        }

        private void CheckCollisions(GameTime gameTime)
        {
            foreach (PlayerAgent p in playerList)
            {
                p.Update(gameTime);
            }
        }

        private void DebugDraw(GraphicsDevice gd, SpriteBatch sb)
        {
            foreach (PlayerAgent p in playerList)
            {
                p.DebugDraw(whiteTexture, gd, sb, testFont, Color.Green);
            }
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
    }
}
