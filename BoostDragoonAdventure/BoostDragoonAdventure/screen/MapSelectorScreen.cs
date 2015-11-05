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
using wickedcrush.manager.gameplay.room;
using wickedcrush.manager.audio;
using wickedcrush.manager.gameplay;

namespace wickedcrush.screen
{
    public class MapSelectorScreen : GameScreen
    {
        //private GameplayManager _gm;
        List<String> mapList;
        int selectionIndex;

        Timer readyTimer;

        private SoundManager _sound;

        public MapSelectorScreen(GameBase game)
        {
            //this._gm = gameplayManager;
            this._sound = game.soundManager;
            Initialize(game);
        }

        public override void Initialize(GameBase g)
        {
            base.Initialize(g);

            exclusiveDraw = true;
            exclusiveUpdate = true;

            readyTimer = new Timer(20);
            readyTimer.start();
            //game.controlsManager = new ControlsManager(g);


            LoadMapListFromAtlas();


            

            //game.mapName = "Temple Halls V4";
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
            mapList = game.mapManager.atlas.Keys.ToList<String>();

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
            if (game.instantAction)
            {
                selectionIndex = 0;
                game.screenManager.AddScreen(new GameplayScreen(game, mapList[selectionIndex]));

            }

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
                    //game.mapName = mapList[selectionIndex];
                    game.screenManager.StartLoading();
                    game.RunOneFrame();
                    game.screenManager.AddScreen(new GameplayScreen(game, mapList[selectionIndex]));

                    
                    
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
