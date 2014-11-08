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

namespace wickedcrush.screen
{
    public class User
    {
        public String name;
        public Controls controls;
        public int id;

        public int selection;

        public Player p;

        public User(String name, Controls controls, int id)
        {
            this.name = name;
            this.controls = controls;
            this.id = id;
            this.selection = -1;
        }
    }

    struct LocalChar
    {
        public String name;
        public String localId;

        public LocalChar(String name, String localId)
        {
            this.name = name;
            this.localId = localId;
        }
    }

    public class PlayerSelect : GameScreen
    {

        //Dictionary<Player, Timer> readyTimer;
        Dictionary<User, Timer> readyTimer;

        List<User> userList = new List<User>();

        List<LocalChar> charList = new List<LocalChar>();

        bool updateCharList = false;

        public PlayerSelect(Game game)
        {
            Initialize(game);
        }

        public override void Initialize(Game g)
        {
            base.Initialize(g);

            exclusiveDraw = true;
            exclusiveUpdate = true;

            LoadLocalChars();

            readyTimer = new Dictionary<User, Timer> ();
        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "";

            foreach (KeyValuePair<User, Timer> pair in readyTimer)
            {
                pair.Value.Update(gameTime);
            }

            if (updateCharList)
            {
                LoadLocalChars();
                updateCharList = false;
            }

            UpdateUsers();
            checkForNewPlayers();

            
            

            foreach (User u in userList)
            {
                if (!readyTimer.ContainsKey(u) || !readyTimer[u].isDone() || u.p == null)
                    return;

                if (u.controls.StartPressed())
                {
                    game.AddScreen(new MapSelector(game));
                    return;
                }

                if (u.controls.SelectPressed())
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
            DrawPlayerSelect(game.spriteBatch, game.testFont);
            game.spriteBatch.DrawString(game.testFont, "Press Start / Enter!", new Vector2(260, 460), Color.White);
        }

        private void DrawPlayerSelect(SpriteBatch sb, SpriteFont f)
        {
            foreach (Player p in game.playerManager.getPlayerList())
            {
                sb.DrawString(f, p.name, new Vector2(p.playerNumber * 100 + 5, 5), Color.White);
            }

            foreach (User u in userList)
            {
                sb.DrawString(f, u.name, new Vector2(u.id * 100 + 5, 5), Color.White);
                DrawCharacterSelect(sb, f, u);
            }
        }

        private void DrawCharacterSelect(SpriteBatch sb, SpriteFont f, User u)
        {
            Color c = Color.White;

            if(u.selection==-1)
                sb.DrawString(f, "New Character", new Vector2(u.id * 100 + 5, 15), Color.Yellow);
            else
                sb.DrawString(f, "New Character", new Vector2(u.id * 100 + 5, 15), Color.White);

            for (int i = 0; i < charList.Count; i++)
            {
                c = Color.White;
                
                if(i==u.selection)
                    c=Color.Yellow;

                sb.DrawString(f, charList[i].name, new Vector2(u.id * 100 + 5, i * 10 + 25), c);
            }
        }

        private void UpdateUsers()
        {
            foreach (User u in userList)
            {
                UpdateUser(u);
            }
        }

        private void UpdateUser(User u)
        {
            if (!readyTimer[u].isDone())
                return;

            if (u.controls.DownPressed())
            {
                u.selection++;
            }

            if (u.controls.UpPressed())
            {
                u.selection--;
            }

            if (u.selection < -1)
                u.selection = charList.Count - 1;
            else if (u.selection > charList.Count - 1)
                u.selection = -1;

            if (u.controls.StartPressed() && u.p == null)
            {
                SelectCharacter(u);
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

        public override void Dispose()
        {

        }

        private void LoadLocalChars()
        {
            charList.Clear();

            if (!Directory.Exists("characters/"))
                Directory.CreateDirectory("characters/");


            string[] filePaths = Directory.GetFiles("characters/");

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
            game.AddScreen(new CharCreation(game, u));
            //u.p = game.playerManager.addNewPlayer(u.name, u.id, u.controls);
        }

        private void loadPlayer(User u)
        {
            u.p = game.playerManager.loadPlayer(charList[u.selection].localId, u.id, u.controls);
        }

        private void addPlayer(String name, Controls controls, int playerNumber) //needs a new home
        {
            Player p = new Player((game.playerManager.getPlayerList().Count + 1) + " " + name, playerNumber, controls, new PersistedStats(15, 15), game.panelFactory);
            p.initializeAgentStats();
            //p.GenerateAgent(new Vector2(12, 320), new Vector2(24, 24), new Vector2(12, 12), true, this);
            game.playerManager.addPlayer(p);
        }

        private void derpReadyTimer(User u)
        {
            if(!readyTimer.ContainsKey(u))
                readyTimer.Add(u, new Timer(20));

            readyTimer[u].resetAndStart();
        }

    }
}
