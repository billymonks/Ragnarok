using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.editor;
using wickedcrush.manager.map.room;

namespace wickedcrush.screen
{
    public class LoadRoomMenuScreen : GameScreen
    {
        public RoomInfo room;

        Dictionary<string, RoomInfo> roomAtlas;
        List<String> roomKeysList;
        int selectionIndex;

        public LoadRoomMenuScreen(Game g, RoomInfo room)
        {
            this.room = room;

            Initialize(g);
        }

        public override void Initialize(Game g)
        {
            base.Initialize(g);

            LoadRoomListFromLocalAtlas();
        }

        private void LoadRoomListFromLocalAtlas()
        {
            roomAtlas = game.mapManager.roomManager.localAtlas;
            roomKeysList = roomAtlas.Keys.ToList<String>();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
