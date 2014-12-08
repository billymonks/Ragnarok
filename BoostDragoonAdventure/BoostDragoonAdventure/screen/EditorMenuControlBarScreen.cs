using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.factory.sprite;
using Microsoft.Xna.Framework;
using wickedcrush.menu.editor;
using wickedcrush.controls;
using wickedcrush.menu.editor.buttonlist;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.editor.tool;
using wickedcrush.menu.input;

namespace wickedcrush.screen
{
    public class EditorMenuControlBarScreen : GameScreen
    {
        private SpriteFactory sf;

        public Vector2 cursorPosition;
        public Vector2 scaledCursorPosition;

        private EditorBarMenu bar;
        private EditorScreen _parent;

        public EditorMenuControlBarScreen(Game g, EditorScreen parent)
        {
            base.Initialize(g);

            sf = new SpriteFactory(g.Content);

            _parent = parent;

            exclusiveDraw = false;
            exclusiveUpdate = true;

            InitializeEditorMenu();

            cursorPosition = _parent.cursorPosition;
            scaledCursorPosition = _parent.scaledCursorPosition;
        }

        private void InitializeEditorMenu()
        {
            Dictionary<string, MenuNode> nodes = new Dictionary<string, MenuNode>();

            bar = new EditorBarMenu(_parent, _parent.cursorPosition);

            Button newButton = new Button(
                sf.createText(new Vector2(0f, 0f), "New", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                e =>
                {
                    _parent.NewRoom();
                    Dispose();
                }
                );

            Button saveButton = new Button(
                sf.createText(new Vector2(0f, 0f), "Save", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                e =>
                {
                    _parent.SaveRoom();
                    game.screenManager.RemoveScreen(this);
                }
                );

            Button authorButton = new Button(
                sf.createText(new Vector2(0f, 0f), "Author", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                e =>
                {
                    _parent.AuthorRoom();
                    game.screenManager.RemoveScreen(this);
                }
                );

            Button renameButton = new Button(
                sf.createText(new Vector2(0f, 0f), "Rename", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                e =>
                {
                    _parent.textInput = new TextInput(_parent.user.c);
                    game.screenManager.RemoveScreen(this);
                }
                );

            Button loadButton = new Button(
                sf.createText(new Vector2(0f, 0f), "Load", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                e =>
                {
                    _parent.LoadRoom();
                    game.screenManager.RemoveScreen(this);
                }
                );

            Button exitButton = new Button(
                sf.createText(new Vector2(0f, 0f), "Exit", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                e =>
                {
                    game.screenManager.RemoveScreen(_parent);
                    game.screenManager.RemoveScreen(this);
                }
                );

            bar.controlBar.Add(newButton);
            bar.controlBar.Add(saveButton);
            bar.controlBar.Add(authorButton);
            bar.controlBar.Add(renameButton);
            bar.controlBar.Add(loadButton);
            bar.controlBar.Add(exitButton);

        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "";


            DebugControls(gameTime);

            UpdateCursorPosition(_parent.user.c);

            bar.Update(gameTime, cursorPosition);
        }

        private void UpdateCursorPosition(Controls c)
        {
            if (c is KeyboardControls)
            {
                UpdateCursorPosition((KeyboardControls)c);
            }
            else
            {
                UpdateCursorPosition((GamepadControls)c);
            }
        }

        private void UpdateCursorPosition(KeyboardControls c)
        {
            cursorPosition.X = c.mousePosition().X;
            cursorPosition.Y = c.mousePosition().Y;

            scaledCursorPosition.X = c.mousePosition().X * (1 / game.debugyscale) - (game.GraphicsDevice.Viewport.Width * 0.5f * (1 / game.debugyscale) - 320);
            scaledCursorPosition.Y = c.mousePosition().Y * (1 / game.debugyscale);

            game.diag += "Cursor Position: " + cursorPosition.X + ", " + cursorPosition.Y + "\n";
            game.diag += "4:3 Cursor Position: " + scaledCursorPosition.X + ", " + scaledCursorPosition.Y + "\n";
        }

        private void UpdateCursorPosition(GamepadControls c)
        {
            float sensitivity = 10f;
            cursorPosition.X += c.LStickXAxis() * sensitivity;
            cursorPosition.Y += c.LStickYAxis() * sensitivity;

            scaledCursorPosition.X = cursorPosition.X * (1 / game.debugyscale) - (game.GraphicsDevice.Viewport.Width * 0.5f * (1 / game.debugyscale) - 320);
            scaledCursorPosition.Y = cursorPosition.Y * (1 / game.debugyscale);


            game.diag += "Cursor Position: " + cursorPosition.X + ", " + cursorPosition.Y + "\n";
            game.diag += "4:3 Cursor Position: " + scaledCursorPosition.X + ", " + scaledCursorPosition.Y + "\n";
        }

        private void DrawMenu()
        {
            bar.Draw(game.spriteBatch);
        }

        private void DrawCursor()
        {
            game.spriteBatch.Draw(_parent.cursorTexture, cursorPosition, Color.LightGreen);

            //game.spriteBatch.DrawCircle(cursorPosition, 90, Color.LightGreen, 1, 32); //cool
        }

        public override void DebugDraw()
        {

        }

        public override void FreeDraw()
        {
            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, Matrix.Identity);

            DrawMenu();
            DrawCursor();

            game.spriteBatch.End();
        }

        public override void Dispose()
        {
            //throw new NotImplementedException();
        }

        private void DebugControls(GameTime gameTime)
        {
            if (_parent.user.c is KeyboardControls)
            {
                KeyboardControls keyboard = (KeyboardControls)_parent.user.c;
                UpdateCursorPosition(keyboard);


                if (keyboard.BoostPressed() || keyboard.SelectPressed())
                {
                    game.screenManager.RemoveScreen(this);
                }

                if (keyboard.ActionPressed())
                {
                    bar.Click();
                }
            }
            else if (_parent.user.c is GamepadControls)
            {
                GamepadControls gamepad = (GamepadControls)_parent.user.c;
                UpdateCursorPosition(gamepad);

                if (gamepad.ItemAPressed())
                {
                    game.screenManager.RemoveScreen(this);
                }

                if (gamepad.InteractPressed())
                {
                    bar.Click();
                }
            }
        }
    }
}
