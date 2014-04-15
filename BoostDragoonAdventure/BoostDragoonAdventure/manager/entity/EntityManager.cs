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

namespace wickedcrush.manager.entity
{
    public class EntityManager : Microsoft.Xna.Framework.GameComponent
    {
        Game g;

        private List<Entity> entityList = new List<Entity>();
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

        public void Update(GameTime gameTime)
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
            entityList.Add(e);
        }

        public void addEntity(PhysicsEntity e)
        {
            entityList.Add(e);
        }

        public void addEntity(Agent e)
        {
            entityList.Add(e);
        }

        public void DebugDraw(GraphicsDevice gd, SpriteBatch sb, Texture2D whiteTexture, SpriteFont testFont)
        {
            foreach (Entity e in entityList)
            {
                e.DebugDraw(whiteTexture, gd, sb, testFont, Color.Green);
            }
        }
    }
}
