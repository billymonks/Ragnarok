using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.screen;
using wickedcrush.screen;

namespace wickedcrush
{
    public class WinGame : GameBase
    {
        public WinGame()
            : base()
        {
            if (fullscreen)
            {
                graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                graphics.IsFullScreen = true;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
                graphics.IsFullScreen = false;
            }

            aspectRatio = (float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;

            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();

            screenManager = new ScreenManager(this, new PlayerSelectScreen(this));
        }
    }
}
