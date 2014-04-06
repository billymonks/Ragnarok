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
using wickedcrush.entity.player;


namespace wickedcrush.component
{
    public class Test : Microsoft.Xna.Framework.GameComponent
    {
        List<Entity> playerList;
        Map testMap;
        SpriteFont testFont;

        Game game;

        public Test(Game game)
            : base(game)
        {
            LoadContent(game);

            this.game = game;
            
            Initialize();

            
        }

        public override void Initialize()
        {
            base.Initialize();

            testMap = new Map(640, 480, "Test Map");
            bool[,] testData = new bool[3,3];
            //testData[0, 0] = true;
            testData[1 , 1] = true;
            testData[2, 2] = true;
            testMap.addLayer(testData, LayerType.WALL);

            playerList = new List<Entity>();
        }

        private void checkAndAdd()
        {
            game.controlsManager.checkAndAddGamepads();
        }

        private void addPlayer(Controls controls)
        {
            playerList.Add(new Player(new Vector2(100, 100), new Vector2(60, 60), new Vector2(30, 30), true, controls));
        }

        private void LoadContent(Game game)
        {
            testFont = game.Content.Load<SpriteFont>("Fonts/TestFont");
        }

        public override void Update(GameTime gameTime)
        {
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
            DrawPlayers(gd, sb);
            

            DrawDiag(gd, sb);
        }

        private void UpdatePlayers(GameTime gameTime)
        {
            List<Entity> newTempList = new List<Entity>(playerList);
            foreach (Player p in playerList)
            {

                newTempList.Remove(p);
                p.CheckCollisions(gameTime, testMap, newTempList);
                p.Update(gameTime);
            }
        }

        private void CheckCollisions(GameTime gameTime)
        {
            foreach (Player p in playerList)
            {
                if (!testMap.predictedLayerCollision(p.body, LayerType.WALL, p.velocity))
                    p.Update(gameTime);
            }
        }

        private void DrawPlayers(GraphicsDevice gd, SpriteBatch sb)
        {
            foreach (Player p in playerList)
            {
                p.DrawBody(gd, sb, testFont, Color.Green);
            }
        }

        private void DrawDiag(GraphicsDevice gd, SpriteBatch sb)
        {
            sb.DrawString(testFont, game.diag, new Vector2(3, 0), Color.Black);
            sb.DrawString(testFont, game.diag, new Vector2(4, 2), Color.Black);
            sb.DrawString(testFont, game.diag, new Vector2(5, 1), Color.White);
            game.diag = "";
        }
    }
}
