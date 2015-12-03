﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.gameplay;
using wickedcrush.player;
using wickedcrush.entity;
using Microsoft.Xna.Framework;
using wickedcrush.controls;
using Com.Brashmonkey.Spriter.player;

namespace wickedcrush.screen.menu
{
    public class MessageDisplayScreen : MenuScreen
    {
        String messageText;
        protected TextEntity message;

        SpriterPlayer window;

        public MessageDisplayScreen(String messageText, GameBase game, GameplayManager gm, Player p)
            : base(game, gm, p)
        {
            this.messageText = messageText;

            message = new TextEntity(messageText, new Vector2(720, 540), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);
            AddText(message);
        }

        public override void Initialize(wickedcrush.GameBase g)
        {
            base.Initialize(g);

            window = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 6, _gm.factory._spriterManager.spriters["hud"].loader);
            window.setAnimation("3line", 0, 0);
            AddSpriter(window);
            
        }

        public override void Dispose()
        {
            game.screenManager.RemoveScreen(this);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            window.update(720, -540);

            if (p.c is KeyboardControls)
                UpdateCursorPosition((KeyboardControls)p.c);

            if (p.c.LaunchMenuPressed() || p.c.InteractPressed())
            {
                Dispose();
            }
        }
    }
}