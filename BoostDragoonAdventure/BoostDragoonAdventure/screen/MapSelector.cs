﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.manager.controls;
using Microsoft.Xna.Framework.Input;
using wickedcrush.utility;
using wickedcrush.player;
using wickedcrush.manager.map.room;
using wickedcrush.manager.audio;

namespace wickedcrush.screen
{
    public class MapSelector : GameScreen
    {
        private MapManager mm;
        List<String> mapList;
        int selectionIndex;

        Timer readyTimer;

        private SoundManager _sound;

        public MapSelector(Game game)
        {
            this.mm = game.mapManager;
            this._sound = game.soundManager;
            Initialize(game);
        }

        public override void Initialize(Game g)
        {
            base.Initialize(g);

            exclusiveDraw = true;
            exclusiveUpdate = true;

            readyTimer = new Timer(20);
            readyTimer.start();
            //game.controlsManager = new ControlsManager(g);


            LoadMapListFromAtlas();

            game.mapName = "Temple Halls V4";
        }

        public override void Update(GameTime gameTime)
        {
            readyTimer.Update(gameTime);

            if (!readyTimer.isDone())
                return;

            DebugControls();
            game.diag = "";
        }

        private void LoadMapListFromAtlas()
        {
            mapList = mm.atlas.Keys.ToList<String>();

        }

        private void LoadMapList()
        {
            mapList = new List<String>();

            string[] files = System.IO.Directory.GetFiles(@"Content\maps\full", "*.xml", SearchOption.AllDirectories);
            selectionIndex = 0;

            for (int i = 0; i < files.Length; i++)
            {
                mapList.Add(files[i]);
            }

            files = System.IO.Directory.GetFiles(@"Content\maps\small", "*.xml", SearchOption.AllDirectories);
            selectionIndex = 0;

            for (int i = 0; i < files.Length; i++)
            {
                mapList.Add(files[i]);
            }
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
                        selectionIndex = mapList.Count - 1;
                }

                if (p.c.DownPressed())
                {
                    _sound.playCue("ping3");
                    selectionIndex++;
                    if (selectionIndex > mapList.Count - 1)
                        selectionIndex = 0;
                }

                if (p.c.StartPressed())
                {
                    game.mapName = mapList[selectionIndex];
                    game.AddScreen(new Gameplay(game));
                    return;
                }

                if (p.c.ItemAPressed())
                {
                    game.mapName = mapList[selectionIndex];
                    game.AddScreen(new Editor(game));
                    return;
                }

                if (p.c.SelectPressed())
                {
                    game.RemoveScreen(this);
                    return;
                }
            }
        }

        public override void Draw()
        {
            //DebugDraw();
        }

        public override void DebugDraw()
        {
            game.GraphicsDevice.Clear(Color.Black);

            if (mapList == null)
                return;

            for (int i = 0; i < mapList.Count; i++)
            {
                if (i == selectionIndex)
                    game.spriteBatch.DrawString(game.testFont, mapList[i], new Vector2(15f, 10f + i * 10f), Color.Yellow);
                else
                    game.spriteBatch.DrawString(game.testFont, mapList[i], new Vector2(15f, 10f + i * 10f), Color.White);
            }
        }

        public override void Dispose()
        {

        }
    }
}
