using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using wickedcrush.map.path;
using wickedcrush.map;
using FarseerPhysics.Factories;
using wickedcrush.stats;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.entity.physics_entity.agent
{
    public class Agent : PhysicsEntity
    {
        Navigator navigator;
        Stack<PathNode> path;
        Entity target;

        public PersistedStats stats;

        public Agent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
            : base(w, pos, size, center, solid)
        {
            Initialize(w, pos, size, center, solid);
        }

        public Agent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, PersistedStats stats)
            : base(w, pos, size, center, solid)
        {
            Initialize(w, pos, size, center, solid);
            this.stats = stats;
        }

        protected virtual void Initialize(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            stats = new PersistedStats(5, 5, 5);
            this.name = "Agent";
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
            FixtureFactory.AttachRectangle(size.X, size.Y, 1f, center, body);
            body.FixedRotation = true;
            body.LinearVelocity = Vector2.Zero;
            body.BodyType = BodyType.Dynamic;
            body.CollisionGroup = (short)CollisionGroup.AGENT;

            body.UserData = this;

            if (!solid)
                body.IsSensor = true;

            FixtureFactory.AttachRectangle(1f, 1f, 1f, center, hotSpot);
            hotSpot.FixedRotation = true;
            hotSpot.LinearVelocity = Vector2.Zero;
            hotSpot.BodyType = BodyType.Dynamic;

        }

        public void activateNavigator(Map m)
        {
            navigator = new Navigator(m, (int)size.X);
        }

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //createPathToTarget();

            body.LinearVelocity /= 1.1f; //friction
            
            if(path != null && path.Count > 0)
                FollowPath();

            HandleCollisions();

            hotSpot.Position = body.Position;

            if (stats.hp <= 0)
                Remove();
        }

        

        protected void FollowPath()
        {
            if (path.Peek().box.Contains(new Point((int)(pos.X), (int)(pos.Y))))
            {
                path.Pop();
            }

            if (path.Count > 0)
            {
                Vector2 v = body.LinearVelocity;

                v.X /= 1.1f;
                v.Y /= 1.1f;

                if (path.Peek().pos.X + path.Peek().gridSize <= pos.X)
                    v.X += -50f;
                else if (pos.X < path.Peek().pos.X)
                    v.X += 50f;

                if (pos.Y < path.Peek().pos.Y)
                    v.Y += 50f;
                else if (path.Peek().pos.Y + path.Peek().gridSize <= pos.Y)
                    v.Y += -50f;

                body.LinearVelocity = v;
            }

            
        }
        #endregion

        public void setTarget(Entity target) //public for testing
        {
            this.target = target;
            createPathToTarget();
        }

        protected void createPathToTarget()
        {
            if (target != null)
            {
                Point start = new Point((int)((pos.X - center.X) / 10), (int)(pos.Y - center.Y) / 10); //convert pos to gridPos for start
                Point goal = new Point((int)((target.pos.X) / 10), (int)(target.pos.Y) / 10); //convert target pos to gridPos for goal //hard coded in 10 for navigator gridSize (half of wall layer gridSize, matches object layer)
                path = navigator.getPath(start, goal);
            }
        }

        protected void createPathToLocation(Point goal)
        {
            Point start = new Point((int)(pos.X / 10), (int)pos.Y / 10); //convert pos to gridPos for start
            path = navigator.getPath(start, goal);
        }

        protected void createPathToLocation(Vector2 loc)
        {
            Point start = new Point((int)(pos.X / 10), (int)pos.Y / 10); //convert pos to gridPos for start
            Point goal = new Point((int)(loc.X / 10), (int)loc.Y / 10); //convert target pos to gridPos for goal //hard coded in 10 for navigator gridSize (half of wall layer gridSize, matches object layer)
            path = navigator.getPath(start, goal);
        }

        protected virtual void HandleCollisions()
        {
            var c = hotSpot.ContactList;
            while(c != null)
            {
                if (c.Contact.IsTouching && c.Other.UserData is LayerType && ((LayerType)c.Other.UserData).Equals(LayerType.DEATH_SOUP))
                    stats.hp = 0;

                c = c.Next;
            }
        }

        public override void DebugDraw(Texture2D tex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            //if (navigator != null)
            //{
                //navigator.DebugDraw(tex, gd, spriteBatch, f);
            //}

            base.DebugDraw(tex, gd, spriteBatch, f, c);
        }

        private void drawPath()
        {

        }
    }
}
