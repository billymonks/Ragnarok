using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.controls;
using Microsoft.Xna.Framework;
using wickedcrush.manager.player;
using wickedcrush.player;

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
            game.controlsManager = new ControlsManager(g);
            game.playerManager = new PlayerManager(game, game.controlsManager);


        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "";
        }

        public override void Draw()
        {
            DebugDraw();
        }

        private void DebugDraw()
        {
            game.GraphicsDevice.Clear(Color.Black);

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

        private void checkAndAdd()
        {
            game.controlsManager.checkAndAddGamepads();
        }

    }
}
