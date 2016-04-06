using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.controls;
using Microsoft.Xna.Framework;
using wickedcrush.manager.player;
using wickedcrush.player;
using wickedcrush.controls;
using Microsoft.Xna.Framework.Input;
using wickedcrush.stats;
using wickedcrush.utility;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using System.IO;
using wickedcrush.entity;

namespace wickedcrush.screen
{
    public class User
    {
        public String name;
        public Controls controls;
        public int id;

        public int selection;

        public Player p;

        public bool ready = false;

        public User(String name, Controls controls, int id)
        {
            this.name = name;
            this.controls = controls;
            this.id = id;
            this.selection = -1;
        }
    }

    class LocalChar
    {
        public String name;
        public String localId;

        public LocalChar(String name, String localId)
        {
            this.name = name;
            this.localId = localId;
        }
    }

    public class PlayerSelectScreen : GameScreen
    {
        Dictionary<User, Timer> readyTimer = new Dictionary<User, Timer>();
        Timer attractTimer = new Timer(1000);
        List<User> userList = new List<User>();
        List<LocalChar> charList = new List<LocalChar>();

        bool updateCharList = true;
        bool rehydrateScreen = false;

        public PlayerSelectScreen(GameBase game)
        {
            Initialize(game);
        }

        public override void Initialize(GameBase g)
        {
            base.Initialize(g);

            exclusiveDraw = true;
            exclusiveUpdate = true;

            foreach (User u in userList)
            {
                u.ready = false;
            }
            
            attractTimer.autoLoop = true;
            attractTimer.resetAndStart();
            

            LoadLocalChars();
            rehydrateScreen = false;
            //AddText(new TextEntity("Press Start / Enter!", new Vector2(300, 300), g.soundManager, g, -1, g.
        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "";

            attractTimer.Update(gameTime);

            if (rehydrateScreen)
            {
                Initialize(game);
            }

            foreach (KeyValuePair<User, Timer> pair in readyTimer)
            {
                pair.Value.Update(gameTime);
            }

            if (updateCharList)
            {
                LoadLocalChars();
                updateCharList = false;
            }

            if (!ReadyTimersDone())
            {
                return;
            }

            UpdateUsers();
            checkForNewPlayers();


            bool ready = true;

            if (userList.Count == 0)
                ready = false;

            foreach (User u in userList)
            {
                if (u.p == null)
                    return;

                if (u.ready == false)
                    ready = false;

            }

            if (ready)
            {
                if(game.debugMode)
                {
                    game.screenManager.AddScreen(new MapSelectorScreen(game));
                }
                else
                {
                    game.screenManager.StartLoading();
                    //game.RunOneFrame();
                    game.screenManager.AddScreen(new GameplayScreen(game, userList[0].p.getStats().getString("home")));
                }
                
                rehydrateScreen = true;
            }
        }

        public override void Draw()
        {
            game.GraphicsDevice.Clear(Color.Black);
            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, game.spriteScale);
            if (userList.Count == 0)
            {
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(1860 - attractTimer.getPercent() * 400, 420), Color.Plum, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(1460 - attractTimer.getPercent() * 400, 420), Color.Plum, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(1060 - attractTimer.getPercent() * 400, 420), Color.Plum, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(660 - attractTimer.getPercent() * 400, 420), Color.Plum, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(260 - attractTimer.getPercent() * 400, 420), Color.Plum, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(-140 - attractTimer.getPercent() * 400, 420), Color.Plum, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(-540 - attractTimer.getPercent() * 400, 420), Color.Plum, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);

                game.spriteBatch.DrawString(game.fonts["Rubik Mono One"], "Blazing Adventure", new Vector2(560, 460), Color.Purple, 0f, new Vector2(300, 0), 1.5f + (float)Math.Sin(attractTimer.getPercentDouble() * Math.PI * 2.0) * 0.005f, SpriteEffects.None, 0f);
                game.spriteBatch.DrawString(game.fonts["Bonbon"], "Act: Zero", new Vector2(960, 500), Color.White, 0f, new Vector2(300, 0), 0.5f, SpriteEffects.None, 0f);

                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(1460 + attractTimer.getPercent() * 400, 520), Color.Plum);
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(1060 + attractTimer.getPercent() * 400, 520), Color.Plum);
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(660 + attractTimer.getPercent() * 400, 520), Color.Plum);
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(260 + attractTimer.getPercent() * 400, 520), Color.Plum);
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(-140 + attractTimer.getPercent() * 400, 520), Color.Plum);
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(-540 + attractTimer.getPercent() * 400, 520), Color.Plum);
                game.spriteBatch.DrawString(game.fonts["Khula"], "Press Enter!", new Vector2(-940 + attractTimer.getPercent() * 400, 520), Color.Plum);
            }
            else
            {
                DrawPlayerSelect(game.spriteBatch, game.testFont);
            }
            game.spriteBatch.End();
        }

        public override void DebugDraw()
        {
            game.GraphicsDevice.Clear(Color.Black);
            DrawPlayerSelect(game.spriteBatch, game.testFont);
            game.spriteBatch.DrawString(game.testFont, "Press Start / Enter!", new Vector2(260, 460), Color.White);
        }

        private void DrawPlayerSelect(SpriteBatch sb, SpriteFont f)
        {

            foreach (User u in userList)
            {
                //Color temp;
                //if (u.ready)
                    //temp = Color.Green;
                //else
                    //temp = Color.White;



                if (u.p == null)
                {
                    //sb.DrawString(game.fonts["Khula"], u.name, new Vector2(u.id * 100 + 5, 5), temp, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    DrawCharacterSelect(sb, f, u);
                }
                //else
                //{
                    //sb.DrawString(game.fonts["Khula"], u.p.name, new Vector2(u.id * 100 + 5, 5), temp, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                //}
                //else

                
            }
        }

        private void DrawCharacterSelect(SpriteBatch sb, SpriteFont f, User u)
        {
            Color c = Color.White;

            if(u.selection==-1)
                sb.DrawString(game.fonts["Khula"], "New Character", new Vector2(u.id * 100 + 150, 150), Color.Purple);
            else
                sb.DrawString(game.fonts["Khula"], "New Character", new Vector2(u.id * 100 + 150, 150), Color.White);

            sb.DrawString(game.fonts["Bonbon"], "Select your character:\nArrow Keys + Enter", new Vector2(650, 450), Color.White);

            for (int i = 0; i < charList.Count; i++)
            {
                c = Color.White;
                
                if(i==u.selection)
                    c = Color.Purple;

                sb.DrawString(game.fonts["Khula"], charList[i].name, new Vector2(u.id * 100 + 150, i * 60 + 250), c);
            }
        }

        private void UpdateUsers()
        {

            foreach (User u in userList)
            {
                UpdateUser(u);
            }
        }

        private bool ReadyTimersDone()
        {
            foreach (User u in userList)
            {
                if (!readyTimer[u].isDone())
                    return false;
            }
            return true;
        }

        private void UpdateUser(User u)
        {
            bool instantPlay = false;

            if (u.p == null)
            {

                if (instantPlay && charList.Count > 0)
                {
                    u.selection = 0;
                    SelectCharacter(u);
                    
                }

                if (u.controls.DownPressed())
                {
                    u.selection++;
                }

                if (u.controls.UpPressed())
                {
                    u.selection--;
                }

            }

            if (u.selection < -1)
                u.selection = charList.Count - 1;
            else if (u.selection > charList.Count - 1)
                u.selection = -1;

            if (u.controls.StartPressed())
            {
                if (u.p == null)
                {
                    SelectCharacter(u);
                }
                else
                {
                    u.ready = true;
                    derpReadyTimer(u);
                }
            }

            if (u.controls.SelectPressed())
            {
                if (u.p != null)
                {
                    game.screenManager.RemoveScreen(this);
                    ClearCharacterSelection(u);
                }
                else
                {
                    if (userList.Count > 1)
                    {
                        userList.Remove(u);
                    }
                    else
                    {
                        game.screenManager.RemoveScreen(this);
                    }

                }
            }
        }

        private void SelectCharacter(User u)
        {
            if (u.selection == -1)
                addNewPlayer(u);
            else
                loadPlayer(u);

            derpReadyTimer(u);
        }

        private void ClearCharacterSelection(User u)
        {
            u.p.Remove();
            u.p = null;
            u.ready = false;
            derpReadyTimer(u);
        }

        public override void Dispose()
        {

        }

        private void LoadLocalChars()
        {
            charList.Clear();

            //if (!Directory.Exists("characters/"))
                //Directory.CreateDirectory("characters/");


            string[] filePaths = Directory.GetFiles(game.characterPath);

            XDocument temp;

            foreach(string s in filePaths)
            {
                temp = XDocument.Load(s);
                charList.Add(new LocalChar(temp.Element("character").Attribute("name").Value, temp.Element("character").Attribute("localId").Value));
            }
        }

        private void checkForNewPlayers() //needs a new home
        {
            if (game.controlsManager.NewGamepadPressed())
            {
                //addPlayer("Gamepad", game.controlsManager.checkAndAddGamepads());
                addUser("Gamepad", game.controlsManager.checkAndAddGamepads());
            }

            if (game.controlsManager.NewKeyboardPressed())
            {
                //addPlayer("Keyboard", game.controlsManager.addKeyboard());
                addUser("Keyboard", game.controlsManager.addKeyboard());
            }
        }

        private void addUser(String name, Controls controls)
        {
            User u = new User(name, controls, userList.Count);
            userList.Add(u);
            derpReadyTimer(u);
        }

        private void addNewPlayer(User u)
        {
            updateCharList = true;
            derpReadyTimer(u);
            game.controlsManager.joinAllowed = false;
            game.screenManager.AddScreen(new CharCreationScreen(game, u));
            
            //u.p = game.playerManager.addNewPlayer(u.name, u.id, u.controls);
        }

        private void loadPlayer(User u)
        {
            u.p = game.playerManager.loadPlayer(charList[u.selection].localId, u.id, u.controls);
            u.ready = true;
        }

        private void derpReadyTimer(User u)
        {
            if(!readyTimer.ContainsKey(u))
                readyTimer.Add(u, new Timer(20));

            readyTimer[u].resetAndStart();
        }

    }
}
