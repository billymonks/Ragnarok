using System;
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

namespace wickedcrush.screen
{
    public class MapSelector : GameScreen
    {
        List<String> mapList;
        int selectionIndex;

        Timer readyTimer;


        public MapSelector(Game game)
        {
            this.game = game;
            
            Initialize(game);
        }

        public override void Initialize(Game g)
        {
            base.Initialize(g);
            readyTimer = new Timer(20);
            readyTimer.start();
            //game.controlsManager = new ControlsManager(g);


            LoadMapList();

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

        private void LoadMapList()
        {
            mapList = new List<String>();

            string[] files = System.IO.Directory.GetFiles(@"maps", "*.xml", SearchOption.TopDirectoryOnly);
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
                    selectionIndex--;
                    if (selectionIndex < 0)
                        selectionIndex = mapList.Count - 1;
                }

                if (p.c.DownPressed())
                {
                    selectionIndex++;
                    if (selectionIndex > mapList.Count - 1)
                        selectionIndex = 0;
                }

                if (p.c.StartPressed())
                {
                    game.mapName = mapList[selectionIndex];
                    game.screenStack.Push(new Gameplay(game));
                    return;
                }

                if (p.c.SelectPressed())
                {
                    game.screenStack.Pop();
                    return;
                }
            }
        }

        public override void Draw()
        {
            DebugDraw();
        }

        private void DebugDraw()
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
