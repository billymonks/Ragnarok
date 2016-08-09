using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.screen;
using wickedcrush.screen;
using wickedcrush.utility.config;
using wickedcrush.screen.menu;

namespace wickedcrush
{
    public class WinGame : GameBase
    {
        public WinGame()
            : base()
        {
            
            if (settings.fullscreen)
            {
                graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                graphics.IsFullScreen = true;
            }
            else
            {
                graphics.PreferredBackBufferWidth = settings.resolution.X;
                graphics.PreferredBackBufferHeight = settings.resolution.Y;
                graphics.IsFullScreen = false;
            }

            aspectRatio = (float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;

            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();

            screenManager = new ScreenManager(this);
            screenManager.AddScreen(new PlayerSelectScreen(this));
        }
    }
}
