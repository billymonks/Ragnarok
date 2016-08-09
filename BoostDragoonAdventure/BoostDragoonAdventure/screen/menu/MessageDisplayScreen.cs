using System;
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
        protected GameplayManager _gm;
        String messageText;
        protected TextEntity message;

        SpriterPlayer window;

        public MessageDisplayScreen(String messageText, GameBase game, GameplayManager gm, Player p)
            : base(game, p)
        {
            this.messageText = messageText;
            this._gm = gm;

            message = new TextEntity(messageText, new Vector2(720, 540), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);
            message.cueName = "Jump7";
            message.SetMaxWidth(840f);
            AddText(message);
        }

        public override void Initialize(wickedcrush.GameBase g)
        {
            base.Initialize(g);

            window = new SpriterPlayer(game.spriterManager.spriters["hud"].getSpriterData(), 6, game.spriterManager.spriters["hud"].loader);
            window.setAnimation("3line", 0, 0);
            AddSpriter(window);

            RemoveSpriter(crosshairSpriter);
            
        }

        public override void Dispose()
        {
            game.screenManager.RemoveScreen(this);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            window.update(720, -540);

                UpdateCursorPosition(p.c);

                if (p.c.LaunchMenuPressed() || p.c.InteractPressed())
            {
                if (message.text.Equals(message.displayedText))
                    Dispose();
                else
                {
                    message.SetSpeed(0);
                }
            }
        }
    }
}
