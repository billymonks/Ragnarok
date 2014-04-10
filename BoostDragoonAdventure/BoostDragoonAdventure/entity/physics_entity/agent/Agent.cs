using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using wickedcrush.map.path;
using wickedcrush.map;

namespace wickedcrush.entity.physics_entity.agent
{
    public class Agent : PhysicsEntity
    {
        Navigator navigator;
        Stack<PathNode> path;

        public Agent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
            : base(w, pos, size, center, solid)
        {
            Initialize(w, pos, size, center, solid);
        }

        private void Initialize(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            this.name = "Agent";
        }

        public void activateNavigator(Map m)
        {
            navigator = new Navigator(m);
        }

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if(path != null)
                FollowPath();
            else
                body.LinearVelocity /= 2f;
        }

        protected void FollowPath()
        {
            if (path.Peek().pos.X + path.Peek().gridSize < pos.X)
                body.LinearVelocity = new Vector2(-100f, 0f);
            else if (pos.X < path.Peek().pos.X)
                body.LinearVelocity = new Vector2(100f, 0f);
            else if (pos.Y < path.Peek().pos.Y)
                body.LinearVelocity = new Vector2(0f, 100f);
            else if (path.Peek().pos.Y + path.Peek().gridSize < pos.Y)
                body.LinearVelocity = new Vector2(0f, -100f);


            if (path.Peek().box.Contains(new Point((int)pos.X, (int)pos.Y)))
            {
                path.Pop();
            }
        }
        #endregion

        public void setTarget(Entity target) //public for testing
        {
            //convert pos to gridPos for start
            //convert target pos to gridPos for goal
            path = navigator.getPath(new Point(), new Point());
        }
    }
}
