using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.entity.physics_entity;
using wickedcrush.entity.physics_entity.agent;
using wickedcrush.entity.physics_entity.agent.enemy;
using wickedcrush.entity.physics_entity.agent.trap.trigger;
using wickedcrush.map.layer;

namespace wickedcrush.manager.entity
{
    public class EntityManager : Microsoft.Xna.Framework.GameComponent
    {
        Game g;

        private List<Entity> entityList = new List<Entity>();
        private List<Entity> addList = new List<Entity>();
        private List<Entity> removeList = new List<Entity>();

        public EntityManager(Game game)
            : base(game)
        {
            g = game;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            updateEntities(gameTime);

            base.Update(gameTime);
        }

        private void updateEntities(GameTime gameTime)
        {
            foreach (Entity e in entityList)
            {
                e.Update(gameTime);

                if (e.readyForRemoval())
                    removeList.Add(e);
            }

            performRemoval();
            performAdd();
        }

        private void performAdd()
        {
            if (addList.Count > 0)
            {
                foreach (Entity e in addList)
                {
                    entityList.Add(e);
                }

                addList.Clear();
            }
        }

        private void performRemoval()
        {
            if (removeList.Count > 0)
            {
                foreach (Entity e in removeList)
                {
                    entityList.Remove(e);
                    
                }

                removeList.Clear();
            }
        }

        public void addEntity(Entity e)
        {
            addList.Add(e);
        }

        public void addEntity(PhysicsEntity e)
        {
            addList.Add(e);
        }

        public void addEntity(Agent e)
        {
            addList.Add(e);
        }

        public void addEntity(Murderer e)
        {
            addList.Add(e);
        }

        public void connectWiring(Layer wiring)
        {   
            foreach (Entity e in entityList)
            {
                if (e is TriggerBase)
                {
                    ((TriggerBase)e).clearWiring();
                    //((TriggerBase)e).connectWiring();
                }
            }
        }

        public void DebugDraw(GraphicsDevice gd, SpriteBatch sb, Texture2D wTex, Texture2D aTex, SpriteFont testFont)
        {
            foreach (Entity e in entityList)
            {
                e.DebugDraw(wTex, aTex, gd, sb, testFont, Color.Green);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            foreach (Entity e in entityList)
            {
                e.Remove();
            }

            entityList.Clear();

            entityList = null;
            addList = null;
            removeList = null;
        }
    }
}
