using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.factory.entity;
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.manager.gameplay.room;
using wickedcrush.manager.gameplay;
using wickedcrush.screen;

namespace wickedcrush.entity.physics_entity.agent.inanimate
{
    public class DestinationAgent : Agent
    {
        private GameBase g;
        private GameplayManager _gm;

        public DestinationAgent(Vector2 pos, Vector2 size, GameplayManager gameplayManager, GameBase g)
            : base(-1, gameplayManager.w, pos, size, new Vector2(size.X / 2f, size.Y / 2f), false, gameplayManager.factory, g.soundManager)
        {
            this.g = g;
            this._gm = gameplayManager;
            this.immortal = true;
        }

        protected override void HandleCollisions()
        {
            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && c.Other.UserData != null
                    && c.Other.UserData is PlayerAgent)
                {
                    g.screenManager.AddScreen(new TestCompleteScreen(g, _gm));
                }

                c = c.Next;
            }
        }
    }
}
