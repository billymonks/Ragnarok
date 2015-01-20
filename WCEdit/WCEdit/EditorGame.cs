using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.screen;
using wickedcrush.manager.screen;
using wickedcrush.player;
using wickedcrush.controls;

namespace WCEdit
{
    public class EditorGame : wickedcrush.GameBase
    {
        private DevMapEditorScreen mapEditScreen;

        public EditorGame() : base()
        {
            
        }

        protected override void Initialize()
        {
            base.Initialize();

            mapEditScreen = new DevMapEditorScreen(this);
            screenManager = new ScreenManager(this, mapEditScreen);

            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }

        public void ToggleGrid()
        {
            mapEditScreen.gridEnabled = !mapEditScreen.gridEnabled;
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            CalculateDimensionScale();
        }
    }
}
