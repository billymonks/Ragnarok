using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.gameplay;
using wickedcrush.controls;
using Com.Brashmonkey.Spriter.player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.entity;
using Microsoft.Xna.Framework.Input;
using wickedcrush.player;
using wickedcrush.utility.config;

namespace wickedcrush.screen.menu
{
    public class MenuScreen : GameScreen
    {
        protected GameplayManager _gm;
        protected Player p;

        SpriterPlayer crosshairSpriter;

        public Vector2 cursorPos = new Vector2(720, 540);
        public Vector2 cursorPosition = Vector2.Zero;
        public Vector2 prevCursorPosition = Vector2.Zero;
        public Vector2 scaledCursorPosition = Vector2.Zero;

        protected Color backgroundColor = new Color(0f, 0f, 0f, 0f);

        protected float marginSize;

        public MenuScreen(GameBase game, GameplayManager gm, Player p)
        {
            this.game = game;
            _gm = gm;
            this.p = p;

            exclusiveUpdate = true;
            exclusiveDraw = false;

            Initialize(game);
        }

        public override void Initialize(wickedcrush.GameBase g)
        {
            base.Initialize(g);

            marginSize = (_gm.camera.fov - 1.3333333f) * 540f;

            crosshairSpriter = new SpriterPlayer(_gm.factory._spriterManager.spriters["cursor"].getSpriterData(), 0, _gm.factory._spriterManager.spriters["cursor"].loader);
            crosshairSpriter.setAnimation("crosshair", 0, 0);

            AddSpriter(crosshairSpriter);
        }

        public override void Dispose()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            crosshairSpriter.setScale(2f);
            crosshairSpriter.update(cursorPos.X, -cursorPos.Y);
            crosshairSpriter.SetDepth(0.03f);

        }

        public void UpdateCursorPosition(Controls c)
        {
            if (c is KeyboardControls && game.settings.controlMode == ControlMode.MouseAndKeyboard)
            {
                prevCursorPosition.X = game.GraphicsDevice.Viewport.Width / 2;
                prevCursorPosition.Y = game.GraphicsDevice.Viewport.Height / 2;

                cursorPosition.X = ((KeyboardControls)c).mousePosition().X;
                cursorPosition.Y = ((KeyboardControls)c).mousePosition().Y;
            }
            else
            {
                prevCursorPosition = Vector2.Zero;
                cursorPosition.X = c.LStickXAxis() * game.settings.mouseSensitivity * 3;
                cursorPosition.Y = c.LStickYAxis() * game.settings.mouseSensitivity * 3;
            }

            cursorPos += (cursorPosition - prevCursorPosition) * new Vector2(1f, (float)(2.0 / Math.Sqrt(2))) * game.settings.mouseSensitivity;

            ConstrainCursorPos();

            if (game.debugMode)
            {
                game.diag = "Cursor Position: " + cursorPosition.X + ", " + cursorPosition.Y + "\n";
                game.diag += "CursorPos: " + cursorPos.X + ", " + cursorPos.Y + "\n";
            }
        }

        private void ConstrainCursorPos()
        {
            if (cursorPos.X < 0 - (_gm.camera.fov - 1.3333333f) * 540f)
            {
                cursorPos.X = 0 - (_gm.camera.fov - 1.3333333f) * 540f;
            }
            else if (cursorPos.X > 1440f + (_gm.camera.fov - 1.3333333f) * 540f)
            {
                cursorPos.X = 1440f + (_gm.camera.fov - 1.3333333f) * 540f;
            }

            if (cursorPos.Y < 0)
            {
                cursorPos.Y = 0;
            }
            else if (cursorPos.Y > 1080)
            {
                cursorPos.Y = 1080;
            }

            if (game.IsActive)
                Mouse.SetPosition(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);

        }

        

        public override void Draw()
        {
            game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, game.spriteScale);
            game.spriteBatch.Draw(game.whiteTexture, new Rectangle((int)(-marginSize), 0, (int)(1440f + marginSize * 2f), 1080), backgroundColor);
            game.spriteBatch.End();


            game.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullCounterClockwise, game.spriterManager.unlitSpriteEffect, game.spriteScale);

            foreach (SpriterPlayer s in spriters)
            {
                game.spriterManager.DrawPlayer(s);
            }

            game.spriteBatch.End();

            game.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullCounterClockwise, null, game.spriteScale);

            foreach (TextEntity t in screenText)
            {
                t.HudDraw(false, false);
            }



            game.spriteBatch.End();
        }

        public override void FreeDraw()
        {

            //gameplayManager.entityManager.FreeDraw();

            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, Matrix.Identity);
            DrawDiag();
            game.spriteBatch.End();
        }
    }
}
