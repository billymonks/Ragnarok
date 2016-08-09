using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.controls;
using wickedcrush.player;
using Microsoft.Xna.Framework;
using wickedcrush.utility;
using System.IO;
using System.Xml.Linq;
using Com.Brashmonkey.Spriter.player;

namespace wickedcrush.screen.menu
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

    public class CharacterSelectScreen : MenuScreen
    {
        Dictionary<User, Timer> readyTimer = new Dictionary<User, Timer>();
        User user;
        List<LocalChar> charList = new List<LocalChar>();

        protected Dictionary<int, SpriterPlayer> characterSlotSpriters;
        protected Dictionary<int, Rectangle> characterBoxes;

        public CharacterSelectScreen(GameBase g)
            : base(g, null)
        {
            Initialize(g);
        }

        public override void Initialize(GameBase g)
        {
            base.Initialize(g);

            exclusiveDraw = true;
            exclusiveUpdate = true;

            characterSlotSpriters = new Dictionary<int, SpriterPlayer>();
            characterBoxes = new Dictionary<int, Rectangle>();

            LoadLocalChars();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateCursorPosition(game.controlsManager.getKeyboard());
            game.diag = "";

            checkForNewUser();

            foreach (KeyValuePair<User, Timer> pair in readyTimer)
            {
                pair.Value.Update(gameTime);
            }

            if (!ReadyTimersDone())
            {
                return;
            }
            
        }

        private void checkForNewUser() //needs a new home
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
            user = new User(name, controls, 0);
        }

        private void LoadLocalChars()
        {
            charList.Clear();

            //if (!Directory.Exists("characters/"))
            //Directory.CreateDirectory("characters/");


            string[] filePaths = Directory.GetFiles(game.characterPath);

            XDocument temp;

            foreach (string s in filePaths)
            {
                temp = XDocument.Load(s);
                charList.Add(new LocalChar(temp.Element("character").Attribute("name").Value, temp.Element("character").Attribute("localId").Value));
            }
        }

        private void LoadCharacterSlots()
        {
            XDocument slots;
            String slotsPath = Path.Combine(game.path, "slots.xml");
            if (System.IO.File.Exists(slotsPath))
            {
                slots = XDocument.Load(slotsPath);
            }
            else
            {

            }
        }

        private void derpReadyTimer(User u)
        {
            if (!readyTimer.ContainsKey(u))
                readyTimer.Add(u, new Timer(20));

            readyTimer[u].resetAndStart();
        }

        private bool ReadyTimersDone()
        {
            if (user == null)
                return false;
            //foreach (User u in userList)
            //{
                if (!readyTimer[user].isDone())
                    return false;
            //}
            return true;
        }
    }

    
}
