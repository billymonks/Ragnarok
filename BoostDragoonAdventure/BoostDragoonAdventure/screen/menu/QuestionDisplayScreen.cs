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
using wickedcrush.eventscript;
using wickedcrush.helper;

namespace wickedcrush.screen.menu
{
    public class QuestionDisplayScreen : MenuScreen
    {
        String messageText;
        protected TextEntity message;

        SpriterPlayer window;

        protected Dictionary<int, TextEntity> answerText;
        protected Dictionary<int, SpriterPlayer> answerSpriters;
        protected Dictionary<int, Rectangle> answerBoxes;

        int highlightedAnswer = -1;

        String key;
        List<AnswerNode> answers;

        int height = 700;

        public QuestionDisplayScreen(String messageText, String key, List<AnswerNode> answers, GameBase game, GameplayManager gm, Player p)
            : base(game, gm, p)
        {
            this.messageText = messageText;
            this.key = key;
            this.answers = answers;

            message = new TextEntity(messageText, new Vector2(720, 540), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);
            AddText(message);

            answerText = new Dictionary<int, TextEntity>();
            answerSpriters = new Dictionary<int, SpriterPlayer>();
            answerBoxes = new Dictionary<int, Rectangle>();

            window = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 6, _gm.factory._spriterManager.spriters["hud"].loader);
            window.setAnimation("3line", 0, 0);
            AddSpriter(window);

            int i = 0;

            foreach (AnswerNode answer in answers)
            {
                TextEntity tempText;
                SpriterPlayer tempSpriter;

                

                tempSpriter = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader);

                answerBoxes.Add(answer.val, new Rectangle((720 - answers.Count * 110) + i * 220, 20 + height, 200, 200));
                answerSpriters.Add(answer.val, tempSpriter);

                tempText = _gm.factory.addText(answer.text, Helper.CastToVector2(answerBoxes[answer.val].Center), -1, Color.White, 1f);

                answerText.Add(answer.val, tempText);

                AddSpriter(tempSpriter);
                AddText(tempText);

                i++;

            }
        }

        public override void Dispose()
        {
            game.screenManager.RemoveScreen(this);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            highlightedAnswer = -1;

            window.update(720, -540);

            UpdateCursorPosition(p.c);

            foreach (KeyValuePair<int, SpriterPlayer> pair in answerSpriters)
            {

                //pair.Value.setAnimation("blank", 0, 0);

                //if (pair.Key < weaponList.Count)
                pair.Value.setAnimation("unselected", 0, 0);

                if (answerBoxes[pair.Key].Contains(Helper.CastToPoint(cursorPos)))
                {
                    highlightedAnswer = pair.Key;

                    pair.Value.setAnimation("selected", 0, 0);
                }


                pair.Value.setScale(2f);
                pair.Value.update(answerBoxes[pair.Key].Center.X, -answerBoxes[pair.Key].Center.Y);
                pair.Value.SetDepth(0.06f);

            }


            if (p.c.LaunchMenuPressed() || p.c.InteractPressed())
            {
                HandleClick();
            }
        }

        private void HandleClick()
        {
            if (highlightedAnswer > -1)
            {
                p.getStats().set(key, highlightedAnswer);
                Dispose();
            }
        }
    }
}
