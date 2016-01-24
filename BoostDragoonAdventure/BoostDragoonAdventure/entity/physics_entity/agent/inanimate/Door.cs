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
    public class Door : Agent
    {
        public Connection connection;

        private GameplayManager _gameplayManager;

        private bool playerOn = false, textAdded = false;

        private TextEntity destinationText;

        public Door(World w, Vector2 pos, Direction facing, Connection connection, GameplayManager gm, EntityFactory factory, SoundManager sound) 
            : base(w, pos, new Vector2(160f, 160f), new Vector2(80f, 80f), false, factory, sound)
        {
            this.facing = facing;
            this._gameplayManager = gm;
            this.connection = connection;
            this.visible = false;
            this.immortal = true;
            this.noCollision = true;

            destinationText = new TextEntity("Next Area: " + connection.mapName, new Vector2((pos.X + gm.camera.cameraPosition.X) / 2f, (pos.Y + gm.camera.cameraPosition.Z) / 2f), sound, factory._game, -1, factory, 1f, false);
            destinationText.center = true;
            destinationText.inScene = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //destinationText.pos = new Vector2((pos.X + _gameplayManager.camera.cameraPosition.X * 0.5f), (pos.Y + _gameplayManager.camera.cameraPosition.Z * 0.1f));
            destinationText.pos = new Vector2(720, 60);

            if (!textAdded && playerOn)
            {
                _gameplayManager._screen.AddText(destinationText);
                textAdded = true;
            }
            else if (textAdded && !playerOn)
            {
                _gameplayManager._screen.RemoveText(destinationText);
                textAdded = false;
            }

            //prevPlayerOn = playerOn;
        }

        protected override void Dispose()
        {
            base.Dispose();

            if(textAdded)
            {
                _gameplayManager._screen.RemoveText(destinationText);
                textAdded = false;
            }
        }

        protected override void HandleCollisions()
        {
            playerOn = false;
            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && c.Other.UserData != null
                    && c.Other.UserData is PlayerAgent)
                {
                    _gameplayManager.activeConnection = this.connection;
                    ((PlayerAgent)c.Other.UserData).player.respawnPoint = this;
                    playerOn = true;
                }
                
                c = c.Next;
            }
        }
    }
}
