using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.manager.audio;
using wickedcrush.factory.entity;
using wickedcrush.display._3d;
using wickedcrush.manager.gameplay;
using Com.Brashmonkey.Spriter.player;
using wickedcrush.controls;
using wickedcrush.entity.physics_entity.agent;
using FarseerPhysics.Dynamics;
using wickedcrush.entity.physics_entity.agent.player;
using Microsoft.Xna.Framework.Input;

namespace wickedcrush.entity
{
    public class CursorEntity : Agent
    {
        GameBase g;
        //EntityFactory factory;

        //public Dictionary<String,SpriterPlayer> sPlayers;
        //public SpriterPlayer bodySpriter;

        public Vector2 cursorPosition;
        public Vector2 scaledCursorPosition;
        public Vector2 prevCursorPosition;

        String cursorText;
        TextEntity textEntity;

        public Entity cursorTarget;

        public SpriterPlayer softTargetSpriter, crosshairSpriter;
        public bool targetLock = false;

        Vector2 playerPos = Vector2.Zero;
        Vector2 cursorPos = Vector2.Zero;

        public CursorEntity(World w, SoundManager sound, GameBase g, EntityFactory factory) //duration time?
            : base(w, Vector2.Zero, new Vector2(20f, 20f), new Vector2(10f, 10f), false, factory, sound)
        {
            this.g = g;
            this.factory = factory;

            cursorPosition = new Vector2(g.GraphicsDevice.Viewport.Width / 2, g.GraphicsDevice.Viewport.Height / 2);
            scaledCursorPosition = new Vector2();
            prevCursorPosition = new Vector2(g.GraphicsDevice.Viewport.Width / 2, g.GraphicsDevice.Viewport.Height / 2);

            this.immortal = true;
            this.noCollision = true;

            this.cursorText = "";
            this.textEntity = factory.addText(cursorText, this.pos, -1, Color.White, 1f, "Rubik Mono One");

            this.spriteScaleAmount = 5f;
            visible = false;
            
            SetupSpriterPlayer();
            Mouse.SetPosition(g.GraphicsDevice.Viewport.Width / 2, g.GraphicsDevice.Viewport.Height / 2);

            factory.AddSpriterToHud(crosshairSpriter);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateCursorPosition(g.controlsManager.getKeyboard());
            UpdateCursorTarget();
            
            this.SetPos(playerPos + cursorPos);
            //ConstrainCursorPos();
            //this.SetPos(this.pos + ((cursorPosition - prevCursorPosition) * 0.01f));
            //this.SetPos(new Vector2(scaledCursorPosition.X + factory._gm.camera.cameraPosition.X, 
                //scaledCursorPosition.Y * 1.4f - 60f + factory._gm.camera.cameraPosition.Y));

            textEntity.pos = this.pos;

            
            //textEntity.textColor = Color.Red;

            //if (g.controlsManager.getKeyboard().MouseClick())
            //{
                //factory.addChest(this.pos);
            //}
        }

        public void SetPlayerPos(Vector2 playerPos)
        {
            this.playerPos = playerPos;
        }

        private void UpdateCursorTarget()
        {
            if (cursorTarget != null)
            {
                //Vector2 spritePos = new Vector2((cursorTarget.pos.X - factory._gm.camera.cameraPosition.X) * 2.25f, (cursorTarget.pos.Y - factory._gm.camera.cameraPosition.Y) * 2.25f * (float)(Math.Sqrt(2) / 2));
                Vector2 spritePos = new Vector2(
                       (cursorTarget.pos.X + cursorTarget.center.X - factory._gm.camera.cameraPosition.X) * (2f / factory._gm.camera.zoom) * 2.25f - 500 * (2f - factory._gm.camera.zoom),
                       ((cursorTarget.pos.Y + cursorTarget.center.Y - factory._gm.camera.cameraPosition.Y - height) * (2f / factory._gm.camera.zoom) * -2.25f * (float)(Math.Sqrt(2) / 2) + 240 * (2f - factory._gm.camera.zoom) - 100)
                       );

                softTargetSpriter.update(spritePos.X, spritePos.Y + 40 + cursorTarget.height * 2.25f * (float)(Math.Sqrt(2) / 2));
                softTargetSpriter.SetDepth(0.06f);

                
            }

            //if (targetLock)
            //{
                //textEntity.text = "Target Locked";
            //}
            //else
            //{
                //textEntity.text = elapsed.TotalMilliseconds.ToString();
            textEntity.text = "";
            //}

            Vector2 cursorPos = new Vector2(
                    (bodies["body"].Position.X + center.X - factory._gm.camera.cameraPosition.X) * (2f / factory._gm.camera.zoom) * 2.25f - 500 * (2f - factory._gm.camera.zoom),
                    ((bodies["body"].Position.Y + center.Y - factory._gm.camera.cameraPosition.Y - height) * (2f / factory._gm.camera.zoom) * -2.25f * (float)(Math.Sqrt(2) / 2) + 240 * (2f - factory._gm.camera.zoom) - 100)
                    );

            crosshairSpriter.setScale(2f);
            crosshairSpriter.update(cursorPos.X, cursorPos.Y);
            crosshairSpriter.SetDepth(0.03f);
        }

        protected override void HandleCollisions()
        {
            bool selectionFound = false;
            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && c.Other.UserData != null
                    && !(c.Other.UserData is PlayerAgent)
                    && (c.Other.UserData is Agent))
                {
                    //if (cursorTarget == null || !targetLock)
                    if(((Agent)c.Other.UserData).targetable)
                    {
                        factory.AddSpriterToHud(softTargetSpriter);
                        cursorTarget = (Entity)c.Other.UserData;
                    }
                    selectionFound = true;
                }

                c = c.Next;
            }

            if (!selectionFound && !targetLock)
            {
                factory.RemoveSpriterFromHud(softTargetSpriter);
                //bodySpriter.setAnimation("spotlight", 0, 0);
                cursorTarget = null;
            }
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("cursor", new SpriterPlayer(factory._spriterManager.spriters["cursor"].getSpriterData(), 0, factory._spriterManager.spriters["cursor"].loader));
            sPlayers.Add("selected", new SpriterPlayer(factory._spriterManager.spriters["cursor"].getSpriterData(), 0, factory._spriterManager.spriters["cursor"].loader));
            sPlayers.Add("crosshair", new SpriterPlayer(factory._spriterManager.spriters["cursor"].getSpriterData(), 0, factory._spriterManager.spriters["cursor"].loader));

            crosshairSpriter = sPlayers["crosshair"];
            crosshairSpriter.setAnimation("crosshair", 0, 0);

            softTargetSpriter = sPlayers["selected"];
            softTargetSpriter.setAnimation("selected", 0, 0);

            bodySpriter = sPlayers["cursor"];
            bodySpriter.setAnimation("spotlight", 0, 0);
            bodySpriter.setFrameSpeed(20);
        }

        private void ConstrainCursorPos()
        {
            if (playerPos.X + cursorPos.X < factory._gm.camera.cameraPosition.X - (factory._gm.camera.fov - 1.3333333f) * 240f)
            {
                cursorPos.X = factory._gm.camera.cameraPosition.X - (factory._gm.camera.fov - 1.3333333f) * 240f - playerPos.X;
            }
            else if (playerPos.X + cursorPos.X > factory._gm.camera.cameraPosition.X + 640f + (factory._gm.camera.fov - 1.3333333f) * 240f)
            {
                cursorPos.X = factory._gm.camera.cameraPosition.X + 640f + (factory._gm.camera.fov - 1.3333333f) * 240f - playerPos.X;
            }

            if (playerPos.Y + cursorPos.Y < factory._gm.camera.cameraPosition.Y - 65f)
            {
                cursorPos.Y = factory._gm.camera.cameraPosition.Y - 65f - playerPos.Y;
            }
            else if (playerPos.Y + cursorPos.Y > factory._gm.camera.cameraPosition.Y + 620)
            {
                cursorPos.Y = factory._gm.camera.cameraPosition.Y + 620 - playerPos.Y;
            }

        }



        public void UpdateCursorPosition(KeyboardControls c)
        {
            prevCursorPosition.X = g.GraphicsDevice.Viewport.Width / 2;
            prevCursorPosition.Y = g.GraphicsDevice.Viewport.Height / 2;
            
            cursorPosition.X = c.mousePosition().X;
            cursorPosition.Y = c.mousePosition().Y;

            scaledCursorPosition.X = c.mousePosition().X * (1 / g.debugyscale) - (g.GraphicsDevice.Viewport.Width * 0.5f * (1 / g.debugyscale) - 320);
            scaledCursorPosition.Y = c.mousePosition().Y * (1 / g.debugyscale);

            g.diag += "Cursor Position: " + cursorPosition.X + ", " + cursorPosition.Y + "\n";
            g.diag += "4:3 Cursor Position: " + scaledCursorPosition.X + ", " + scaledCursorPosition.Y + "\n";
            g.diag += "Cursor World Position: " + (pos.X + center.X) + ", " + (pos.Y + center.Y) + "\n";
            g.diag += "Camera Position: " + factory._gm.camera.cameraPosition.X + ", " + factory._gm.camera.cameraPosition.Y + "\n";
            g.diag += "Camera Target: " + factory._gm.camera.cameraTarget.X + ", " + factory._gm.camera.cameraTarget.Y + "\n";
            g.diag += "Camera Difference: " + factory._gm.camera.cameraDifference + "\n";
            g.diag += "Camera Shake: " + factory._gm.camera.screenShakeAmount + "\n";
            g.diag += "Le Cursor Pos: " + cursorPos.X + ", " + cursorPos.Y + "\n";
            g.diag += "FOV: " + factory._gm.camera.fov + "\n";
            //g.diag += "Cursor Position: " + cursorPosition.X + ", " + cursorPosition.Y + "\n";
            //g.diag += "Prev Cursor Position: " + prevCursorPosition.X + ", " + prevCursorPosition.Y + "\n";

            //pos = scaledCursorPosition;
            

            cursorPos += (cursorPosition - prevCursorPosition) * new Vector2(1f, (float)(2.0/Math.Sqrt(2))) * factory._game.settings.mouseSensitivity;

            ConstrainCursorPos();

            if(factory._game.IsActive)
            Mouse.SetPosition(g.GraphicsDevice.Viewport.Width / 2, g.GraphicsDevice.Viewport.Height / 2);

            
        }

        protected override void Dispose()
        {
            factory._gm._screen.RemoveSpriter(softTargetSpriter);
            factory._gm._screen.RemoveSpriter(crosshairSpriter);
            base.Dispose();
        }
    }
}
