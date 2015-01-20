using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.factory.sprite;
using wickedcrush.display.primitives;
using wickedcrush.manager.gameplay;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.screen
{
    public class TestCompleteScreen : GameScreen
    {
        private SpriteFactory sf;

        public Vector2 cursorPosition;
        public Vector2 scaledCursorPosition;

        public GameplayManager _gameplayManager;

        public TestCompleteScreen(GameBase g, GameplayManager gameplayManager)
        {
            base.Initialize(g);

            sf = new SpriteFactory(g.Content);

            exclusiveDraw = false;
            exclusiveUpdate = true;

            _gameplayManager = gameplayManager;

            gameplayManager._screen.SetRoomReady();
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

        public override void FreeDraw()
        {
            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, Matrix.Identity);

            DrawDiag();

            game.spriteBatch.End();
        }

        public override void Dispose()
        {
            game.screenManager.RemoveScreen(this);
        }
    }
}
