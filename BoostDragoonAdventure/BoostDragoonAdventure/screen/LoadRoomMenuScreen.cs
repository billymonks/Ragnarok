﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.editor;
using wickedcrush.manager.gameplay.room;
using wickedcrush.player;
using wickedcrush.manager.audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.screen
{
    public class LoadRoomMenuScreen : GameScreen
    {
        public SoundManager _sound;
        public RoomManager _rm;
        RoomInfo room;

        Dictionary<string, RoomInfo> roomAtlas = new Dictionary<string, RoomInfo>();
        List<String> roomKeysList;
        int selectionIndex;

        public LoadRoomMenuScreen(GameBase g, RoomInfo room)
        {
            this._rm = g.roomManager;
            this.room = room;
            this._sound = g.soundManager;

            Initialize(g);
        }

        public override void Initialize(GameBase g)
        {
            base.Initialize(g);

            exclusiveDraw = false;
            exclusiveUpdate = true;

            LoadRoomListFromLocalAtlas();
        }

        private void LoadRoomListFromLocalAtlas()
        {
            roomAtlas.Clear();

            //_rm.LoadLocalAtlas();

            foreach (KeyValuePair<string, RoomInfo> pair in _rm.localAtlas)
            {
                if (pair.Value.globalId == -1)
                    roomAtlas.Add(pair.Key, pair.Value);
            }

            roomKeysList = roomAtlas.Keys.ToList<String>();
        }

        public override void Dispose()
        {
            
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            DebugControls();
        }

        private void DebugControls()
        {
            foreach (Player p in game.playerManager.getPlayerList())
            {
                if (p.c.UpPressed())
                {
                    _sound.playCue("ping3");
                    selectionIndex--;
                    if (selectionIndex < 0)
                        selectionIndex = roomKeysList.Count - 1;
                }

                if (p.c.DownPressed())
                {
                    _sound.playCue("ping3");
                    selectionIndex++;
                    if (selectionIndex > roomKeysList.Count - 1)
                        selectionIndex = 0;
                }

                if (p.c.StartPressed())
                {
                    room.globalId = roomAtlas[roomKeysList[selectionIndex]].globalId;
                    room.localId = roomAtlas[roomKeysList[selectionIndex]].localId;
                    room.roomName = roomAtlas[roomKeysList[selectionIndex]].roomName;
                    room.creatorName = roomAtlas[roomKeysList[selectionIndex]].creatorName;
                    room.readyToLoad = true;
                    game.screenManager.RemoveScreen(this);
                    return;
                }

                if (p.c.SelectPressed())
                {
                    game.screenManager.RemoveScreen(this);
                    return;
                }
            }
        }

        public override void Draw()
        {
            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, game.debugSpriteScale);
            DebugDraw();
            game.spriteBatch.End();
        }

        public override void DebugDraw()
        {
            game.GraphicsDevice.Clear(Color.Black);

            if (roomKeysList == null)
                return;

            for (int i = 0; i < roomKeysList.Count; i++)
            {
                if (i == selectionIndex)
                    game.spriteBatch.DrawString(game.testFont, roomAtlas[roomKeysList[i]].roomName, new Vector2(150f, 100f + i * 10f), Color.Yellow);
                else
                    game.spriteBatch.DrawString(game.testFont, roomAtlas[roomKeysList[i]].roomName, new Vector2(150f, 100f + i * 10f), Color.White);
            }
        }
    }
}
