using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.controls;
using Microsoft.Xna.Framework;
using wickedcrush.manager.player;
using wickedcrush.player;
using wickedcrush.controls;
using Microsoft.Xna.Framework.Input;
using wickedcrush.stats;

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
            //DebugDraw();
        }

        public override void DebugDraw()
        {
            game.GraphicsDevice.Clear(Color.Black);
            game.playerManager.DrawPlayerSelect(game.spriteBatch, game.testFont);
            game.spriteBatch.DrawString(game.testFont, "Press Start / Enter!", new Vector2(260, 460), Color.White);
        }

        public override void Dispose()
        {

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
            Player p = new Player((game.playerManager.getPlayerList().Count + 1) + " " + name, game.playerManager.getPlayerList().Count, controls, new PersistedStats(15, 15), game.panelFactory);
            //p.GenerateAgent(new Vector2(12, 320), new Vector2(24, 24), new Vector2(12, 12), true, this);
            game.playerManager.addPlayer(p);

        }

    }
}
