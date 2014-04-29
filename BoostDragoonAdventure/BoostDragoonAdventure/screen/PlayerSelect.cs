﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.controls;
using Microsoft.Xna.Framework;
using wickedcrush.manager.player;
using wickedcrush.player;
using wickedcrush.controls;
using Microsoft.Xna.Framework.Input;

namespace wickedcrush.screen
{
    public class PlayerSelect : GameScreen
    {

        public PlayerSelect(Game game)
        {
            this.game = game;
            
            Initialize(game);
        }

        public override void Initialize(Game g)
        {
            base.Initialize(g);
        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "";
            checkForNewPlayers();

            foreach (Player p in game.playerManager.getPlayerList())
            {
                if (p.c.StartPressed())
                {
                    game.screenStack.Push(new MapSelector(game));
                    return;

                }


                if (p.c.SelectPressed())
                {
                    game.screenStack.Pop();
                    return;
                }

            }
        }

        public override void Draw()
        {
            DebugDraw();
        }

        private void DebugDraw()
        {
            game.GraphicsDevice.Clear(Color.Black);
            game.playerManager.DrawPlayerSelect(game.spriteBatch, game.testFont);
        }

        public override void Dispose()
        {

        }

        private void DrawPlayerHud()
        {
            foreach (Player p in game.playerManager.getPlayerList())
            {
                game.spriteBatch.DrawString(game.testFont, p.name + "\nHP: " + p.getStats().hp + "/" + p.getStats().maxHP, new Vector2(p.playerNumber * 100 + 5, 5), Color.White);
            }
        }

        private void checkForNewPlayers() //needs a new home
        {
            if (game.controlsManager.gamepadPressed())
            {
                addPlayer("Gamepad", game.controlsManager.checkAndAddGamepads());
            }

            if (game.controlsManager.keyboardPressed())
            {
                addPlayer("Keyboard", game.controlsManager.addKeyboard());
            }
        }

        private void addPlayer(String name, Controls controls) //needs a new home
        {
            Player p = new Player((game.playerManager.getPlayerList().Count + 1) + " " + name, game.playerManager.getPlayerList().Count, controls);
            //p.GenerateAgent(new Vector2(12, 320), new Vector2(24, 24), new Vector2(12, 12), true, this);
            game.playerManager.addPlayer(p);

        }

    }
}