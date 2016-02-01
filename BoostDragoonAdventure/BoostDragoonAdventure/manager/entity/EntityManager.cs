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
using wickedcrush.display._3d;
using wickedcrush.helper;
using wickedcrush.manager.gameplay;

namespace wickedcrush.manager.entity
{
    public class EntityManager : Microsoft.Xna.Framework.GameComponent
    {
        GameBase g;

        public List<Entity> entityList = new List<Entity>();
        private List<Entity> addList = new List<Entity>();
        private List<Entity> removeList = new List<Entity>();

        private List<Entity> addedList = new List<Entity>();

        int MAX_SIMULTANEOUS_ADD = 5;

        public EntityManager(GameBase game)
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
            
            discover();
            
            updateEntities(gameTime);
            

            base.Update(gameTime);
        }

        private void updateEntities(GameTime gameTime)
        {
            foreach (Entity e in entityList)
            {
                if (e.readyForRemoval())
                    removeList.Add(e);
                else
                    e.Update(gameTime);
            }

            performRemoval();
            performAdd();
        }

        private void discover()
        {
            DepthSort();
            for (int i = 0; i < entityList.Count; i++)
            {
                if(entityList[i] is Agent)
                    ((Agent)entityList[i]).proximity.Clear();
            }
            for (int i = 0; i < entityList.Count; i++)
            {
                if(entityList[i] is Agent /*&& ((Agent)entityList[i]).activeRange != 0f*/)
                {
                    

                    for(int j = i+1; j < entityList.Count; j++)
                    {
                        float distance = Helper.getDistance(entityList[i].pos + entityList[i].center, entityList[j].pos + entityList[j].center);

                        if (((Agent)entityList[i]).activeRange > distance)
                            ((Agent)entityList[i]).proximity.Add(entityList[j]);

                        if (entityList[j] is Agent && ((Agent)entityList[j]).activeRange > distance)
                            ((Agent)entityList[j]).proximity.Add(entityList[i]);
                    }
                }
            }
        }

        private void performAdd()
        {

            if (addList.Count > 0)
            {
                for (int i = 0; i < addList.Count && i < MAX_SIMULTANEOUS_ADD; i++)
                {
                    entityList.Add(addList[i]);
                    addedList.Add(addList[i]);
                }

                foreach (Entity e in addedList)
                {
                    addList.Remove(e);
                }

                //foreach (Entity e in addList)
                //{
                    //entityList.Add(e);
                //}

                //addList.Clear();
                addedList.Clear();
            }
        }

        private void performRemoval()
        {
            if (removeList.Count > 0)
            {
                foreach (Entity e in removeList)
                {
                    e.Remove();
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

        public void addEntity(KnightEnemy e)
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

        public void Draw(bool depthPass, Dictionary<string, PointLightStruct> lightList, GameplayManager gameplay)
        {
            
            foreach (Entity e in entityList)
            {
                e.Draw(depthPass, lightList, gameplay);
            }
        }

        public void DepthSort()
        {
            entityList.Sort();
        }

        public void DebugDraw(GraphicsDevice gd, SpriteBatch sb, Texture2D wTex, Texture2D aTex, SpriteFont testFont, Camera camera)
        {
            foreach (Entity e in entityList)
            {
                e.DebugDraw(wTex, aTex, gd, sb, testFont, Color.Green, camera);
            }
        }

        public void FreeDraw()
        {
            g.spriteBatch.Begin();
            foreach (Entity e in entityList)
            {
                e.FreeDraw();
            }
            g.spriteBatch.End();
        }

        public void RemoveAll()
        {
            foreach (Entity e in entityList)
            {
                e.Remove();
            }

            entityList.Clear();
            addList.Clear();
            removeList.Clear();
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
