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
using Com.Brashmonkey.Spriter.player;

namespace wickedcrush.entity.physics_entity.agent.inanimate
{
    public class Door : Agent
    {
        public String destinationMapName;
        public int myId, destinationId;

        private GameplayManager _gameplayManager;

        private bool playerOn = false, textAdded = false;

        private TextEntity promptText;

        public Door(World w, Vector2 pos, Vector2 size, String destinationMapName, int myId, int destinationId, GameplayManager gm, EntityFactory factory, SoundManager sound)
            : base(myId, w, pos, size, size / 2f, false, factory, sound)
        {
            this.facing = Direction.East;
            this._gameplayManager = gm;
            this.destinationMapName = destinationMapName;
            this.myId = myId;
            this.destinationId = destinationId;
            //this.visible = false;
            this.immortal = true;
            this.noCollision = true;

            promptText = new TextEntity("Enter", pos+center, sound, factory._game, -1, factory, 1f, false);
            promptText.alignment = TextAlignment.Center;
            promptText.inScene = false;

            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //destinationText.pos = new Vector2((pos.X + _gameplayManager.camera.cameraPosition.X * 0.5f), (pos.Y + _gameplayManager.camera.cameraPosition.Z * 0.1f));
            promptText.pos = new Vector2(720, 60);

            if (!textAdded && playerOn)
            {
                _gameplayManager._screen.AddText(promptText);
                textAdded = true;
            }
            else if (textAdded && !playerOn)
            {
                _gameplayManager._screen.RemoveText(promptText);
                textAdded = false;
            }

            if(playerOn && _gameplayManager._playerManager.getPlayerList()[0].c.InteractPressed()){
                if (textAdded)
                {
                    _gameplayManager._screen.RemoveText(promptText);
                }
                _gameplayManager.TraverseDoor(this);
            }

            //UpdateSpriters();

            //prevPlayerOn = playerOn;
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("you", new SpriterPlayer(factory._spriterManager.spriters["you"].getSpriterData(), 0, factory._spriterManager.spriters["you"].loader));
            bodySpriter = sPlayers["you"];
            bodySpriter.setFrameSpeed(20);
            drawBody = false;

            sPlayers.Add("shadow", new SpriterPlayer(factory._spriterManager.spriters["shadow"].getSpriterData(), 0, factory._spriterManager.spriters["shadow"].loader));
            shadowSpriter = sPlayers["shadow"];
            shadowSpriter.setAnimation("still", 0, 0);
            shadowSpriter.setFrameSpeed(20);
            drawShadow = false;

            //AddAngledElement("Magazine", "shapes", "grey", 0, new Vector3(0f, 5f, 0f) * 1, 0, 1, 0f, new Vector3(0f, 4f, -3f) * 0);
            AddAngledElement("Door", "environment", "closed", 0, new Vector3(0f, 0f, 0f) * 1, 0, 1f, 0f, new Vector3(0f, 4f, -3f) * 0);
            
            //}
            //else
            //{
                //InitializeHumanoidSprites(torsoScale, armWidth, legWidth, armStance, legStance, spineStance);
                //SetCostume(2);
            //}

        }

        protected override void Dispose()
        {
            base.Dispose();

            if(textAdded)
            {
                _gameplayManager._screen.RemoveText(promptText);
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
                    //_gameplayManager.activeConnection = this.connection;
                    //((PlayerAgent)c.Other.UserData).player.respawnPoint = this;
                    playerOn = true;
                }
                
                c = c.Next;
            }
        }
    }
}
