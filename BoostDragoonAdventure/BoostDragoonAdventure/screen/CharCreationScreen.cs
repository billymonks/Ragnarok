using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using wickedcrush.utility;
using System.Xml.Linq;
using wickedcrush.menu.input;
using Microsoft.Xna.Framework;
using wickedcrush.display.primitives;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.screen
{
    public class CharCreationScreen : GameScreen
    {
        Dictionary<User, Timer> readyTimer;
        List<LocalChar> charList = new List<LocalChar>();

        private TextInput textInput;
        User u;

        public CharCreationScreen(GameBase g, User u)
        {
            this.u = u;
            Initialize(g);
        }

        public override void Initialize(GameBase g)
        {
            base.Initialize(g);

            exclusiveDraw = false;
            exclusiveUpdate = true;

            LoadLocalChars();

            readyTimer = new Dictionary<User, Timer>();
            String localId;
            do
            {
                localId = Guid.NewGuid().ToString();
            } while (File.Exists("characters/" + localId + ".xml"));

            textInput = new TextInput(u.controls);
        }

        

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            game.diag = "Enter name for user id " + u.id + ": " + u.name + ".";

            foreach (KeyValuePair<User, Timer> pair in readyTimer)
            {
                pair.Value.Update(gameTime);
            }

            UpdateTextInput(gameTime);
        }

        public void UpdateTextInput(GameTime gameTime)
        {
            if (textInput == null)
                return;

            textInput.Update(gameTime);

            if (textInput.finished)
            {
                //play a funny sound
                u.name = textInput.getText();
                u.p = game.playerManager.addNewPlayer(u.name, u.id, u.controls);
                u.ready = true;
                textInput = null;
                Dispose();
                return;
            }

            if (textInput.cancelled)
            {
                //play a funny sound
                textInput = null;
                Dispose();
                return;
            }
        }

        private void LoadLocalChars()
        {
            if (!Directory.Exists("characters/"))
                Directory.CreateDirectory("characters/");


            string[] filePaths = Directory.GetFiles("characters/");

            XDocument temp;

            foreach (string s in filePaths)
            {
                temp = XDocument.Load(s);
                charList.Add(new LocalChar(temp.Element("character").Attribute("name").Value, temp.Element("character").Attribute("localId").Value));
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
            DrawTextInput();
        }

        public override void FreeDraw()
        {
            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, Matrix.Identity);

            DrawDiag();

            game.spriteBatch.End();
        }

        private void DrawDiag()
        {

            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(2, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(2, 3), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 3), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(4, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(4, 3), Color.Black);


            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 2), Color.White);

        }

        private void DrawTextInput()
        {
            if (textInput != null)
                textInput.DebugDraw(game.spriteBatch, game.testFont);
        }

        public override void Dispose()
        {
            game.screenManager.RemoveScreen(this);
        }
    }
}
