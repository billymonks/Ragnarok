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


namespace wickedcrush.component
{
    public class Test : Microsoft.Xna.Framework.GameComponent
    {
        List<Entity> entityList; //not including players
        List<Entity> removeList;
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

            testMap = new Map("Temple Halls V4", w);

            entityList = new List<Entity>();
            removeList = new List<Entity>();
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

        

        private void LoadContent(Game game)
        {
            testFont = game.Content.Load<SpriteFont>("Fonts/TestFont");
        }

        public override void Update(GameTime gameTime)
        {
            w.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));

            game.diag = "";

            
            game.playerManager.Update(gameTime, w);
            
            UpdateEntities(gameTime);

            game.controlsManager.Update(gameTime);

            DebugControls();

            base.Update(gameTime);
        }

        public void Draw(GraphicsDevice gd, SpriteBatch sb)
        {
            
            testMap.drawMap(gd, sb, testFont);
            DebugDraw(gd, sb);
            DrawPlayerHud(gd, sb);
            
        }

        private void UpdateEntities(GameTime gameTime) // move to player manager
        {
            foreach (Agent a in entityList)
            {
                a.Update(gameTime);
                
                if (a.remove)
                    removeList.Add(a);
            }
            
            performRemoval();
        }

        private void performRemoval()
        {
            if (removeList.Count > 0)
            {
                foreach (Agent a in removeList)
                {
                    entityList.Remove(a);
                }

                removeList.Clear();
            }
        }

        private void DebugDraw(GraphicsDevice gd, SpriteBatch sb)
        {
            foreach (Entity e in entityList)
            {
                e.DebugDraw(whiteTexture, gd, sb, testFont, Color.Green);
            }

            game.playerManager.DebugDraw(gd, sb, whiteTexture, testFont);
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
            foreach (Player p in game.playerManager.getPlayerList())
            {
                sb.DrawString(testFont, p.name + "\nHP: " + p.getStats().hp + "/" + p.getStats().maxHP, new Vector2(p.playerNumber * 100 + 5, 5), Color.White);
            }
        }

        private void DebugControls()
        {
            if (game.controlsManager.debugControls.KeyPressed(Keys.P))
            {
                Agent f = new Agent(w, new Vector2(600, 130), new Vector2(24, 24), new Vector2(12, 12), true);
                f.activateNavigator(testMap);
                if (game.playerManager.getPlayerList().Count > 0)
                    f.setTarget(game.playerManager.getPlayerList()[0].getAgent());
                entityList.Add(f);
            }

            if (game.controlsManager.debugControls.KeyPressed(Keys.O))
            {
                Agent f = new Agent(w, new Vector2(600, 130), new Vector2(8, 8), new Vector2(4f, 4f), true);
                f.activateNavigator(testMap);
                if (game.playerManager.getPlayerList().Count > 0)
                    f.setTarget(game.playerManager.getPlayerList()[0].getAgent());
                entityList.Add(f);
            }
        }
    }
}
