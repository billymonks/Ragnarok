using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.screen
{
    public class LoadingScreen : GameScreen
    {
        public LoadingScreen(GameBase g)
        {
            Initialize(g);
        }
        public override void Initialize(GameBase g)
        {
            base.Initialize(g);

            exclusiveUpdate = false;
            exclusiveDraw = true;
        }

        public override void FullScreenDraw() 
        { 
        
        }

        public override void DebugDraw()
        {
            game.GraphicsDevice.Clear(Color.Black);
            game.spriteBatch.DrawString(game.testFont, "Loading!", new Vector2(260, 460), Color.White);
        }

        public override void Dispose()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
