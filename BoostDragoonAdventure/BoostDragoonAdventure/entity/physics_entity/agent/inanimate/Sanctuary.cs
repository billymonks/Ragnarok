using Com.Brashmonkey.Spriter.player;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.factory.entity;
using wickedcrush.manager.audio;
using wickedcrush.manager.gameplay;
using wickedcrush.manager.map;

namespace wickedcrush.entity.physics_entity.agent.inanimate
{
    public class Sanctuary : Agent
    {
        private GameplayManager _gameplayManager;

        private bool activeHome = false;

        public Sanctuary(World w, Vector2 pos, GameplayManager gm, EntityFactory factory, SoundManager sound)
            : base(-1, w, pos, new Vector2(30f, 30f), new Vector2(15f, 15f), false, factory, sound)
        {
            this._gameplayManager = gm;
            this.visible = true;
            this.immortal = true;
            this.noCollision = true;

            this.name = "Sanctuary";

            this.spriteScaleAmount = 100f;

            if(gm._playerManager.getPlayerList()[0].getStats().getString("home").Equals(gm.map.name))
            {
                activeHome = true;
            }
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("trap", new SpriterPlayer(factory._spriterManager.spriters["trap"].getSpriterData(), 0, factory._spriterManager.spriters["trap"].loader));
            bodySpriter = sPlayers["trap"];
            bodySpriter.setFrameSpeed(20);

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
                    ((PlayerAgent)c.Other.UserData).stats.set("home", _gameplayManager.map.name);
                    activeHome = true;
                    //_gameplayManager.activeConnection = this.connection;
                    //((PlayerAgent)c.Other.UserData).player.respawnPoint = this;
                }
                
                c = c.Next;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (activeHome)
            {
                bodySpriter.setAnimation("pressed", 0, 0);
            }
            else
            {
                bodySpriter.setAnimation("unpressed", 0, 0);
            }
        }
    }
}
