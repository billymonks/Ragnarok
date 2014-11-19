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

namespace wickedcrush.entity.physics_entity.agent.inanimate
{
    public class Door : Agent
    {
        public Connection connection;

        private GameplayManager _gameplayManager;

        public Door(World w, Vector2 pos, Direction facing, Connection connection, GameplayManager gm, EntityFactory factory, SoundManager sound) 
            : base(w, pos, new Vector2(80f, 80f), new Vector2(40f, 40f), false, factory, sound)
        {
            this.facing = facing;
            this._gameplayManager = gm;
            this.connection = connection;
            this.visible = false;
            
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
                    _gameplayManager.activeConnection = this.connection;
                }
                
                c = c.Next;
            }
        }
    }
}
