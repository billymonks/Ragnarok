using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.factory.sprite;
using wickedcrush.display.primitives;
using wickedcrush.manager.gameplay;

namespace wickedcrush.screen
{
    public class TestCompleteScreen : GameScreen
    {
        private SpriteFactory sf;

        public Vector2 cursorPosition;
        public Vector2 scaledCursorPosition;

        public GameplayManager _gameplayManager;

        public TestCompleteScreen(Game g, GameplayManager gameplayManager)
        {
            base.Initialize(g);

            sf = new SpriteFactory(g.Content);

            exclusiveDraw = false;
            exclusiveUpdate = true;

            _gameplayManager = gameplayManager;

            if (gameplayManager._screen.roomToTest != null)
            {
                gameplayManager._screen.roomToTest.readyToAuthor = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "Room Complete!\nPress Start/Enter to return to the Editor!";

            if (game.controlsManager.StartPressed())
            {
                _gameplayManager._screen.Dispose();
                Dispose();
            }
        }

        public override void DebugDraw()
        {
            
        }

        public override void Dispose()
        {
            game.screenManager.RemoveScreen(this);
        }
    }
}
