using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.factory.sprite;
using wickedcrush.display.primitives;

namespace wickedcrush.screen
{
    public class TestCompleteScreen : GameScreen
    {
        private SpriteFactory sf;

        public Vector2 cursorPosition;
        public Vector2 scaledCursorPosition;

        public TestCompleteScreen(Game g)
        {
            base.Initialize(g);

            sf = new SpriteFactory(g.Content);

            exclusiveDraw = false;
            exclusiveUpdate = true;
        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "Room Complete!\nPress Start/Enter to return to the Editor!";
        }

        public override void DebugDraw()
        {
            
        }

        public override void Dispose()
        {
            
        }
    }
}
