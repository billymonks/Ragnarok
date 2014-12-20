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
using wickedcrush.helper;
using wickedcrush.utility;
using wickedcrush.behavior;
using wickedcrush.factory.entity;
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.utility.trigger;
using wickedcrush.inventory;
using wickedcrush.manager.audio;
using wickedcrush.display._3d;
using wickedcrush.entity.physics_entity.agent.attack;
using Com.Brashmonkey.Spriter.player;
using wickedcrush.display.spriter;

namespace wickedcrush.entity.physics_entity.agent
{

    public class Agent : PhysicsEntity
    {
        protected Navigator navigator;
        protected Stack<PathNode> path;
        protected Dictionary<String, Timer> timers;
        protected Dictionary<String, Trigger> triggers;
        protected StateMachine sm;
        protected EntityFactory factory;
        protected SpriterManager _spriterManager;

        public List<Entity> proximity;
        protected Entity target;

        public PersistedStats stats;

        protected float speed = 50f;
        public float activeRange = 0f;
        //protected bool strafe = false;

        public bool staggered = false;

        public SpriterPlayer sPlayer;

        public Agent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, SoundManager sound)
            : base(w, pos, size, center, solid, sound)
        {
            _spriterManager = factory._spriterManager;
            Initialize(new PersistedStats(5, 5), factory);
        }

        public Agent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, PersistedStats stats, SoundManager sound)
            : base(w, pos, size, center, solid, sound)
        {
            _spriterManager = factory._spriterManager;
            Initialize(stats, factory);
        }

        private void Initialize(PersistedStats stats, EntityFactory factory)
        {
            this.factory = factory;
            this.stats = stats;

            stats.set("staggerDistance", 0);

            timers = new Dictionary<String, Timer>();
            triggers = new Dictionary<String, Trigger>();
            proximity = new List<Entity>();
            this.name = "Agent";

            SetupSpriterPlayer();
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
            FixtureFactory.AttachCircle(size.X / 2, 1f, bodies["body"], center);
            bodies["body"].FixedRotation = true;
            bodies["body"].LinearVelocity = Vector2.Zero;
            bodies["body"].BodyType = BodyType.Dynamic;
            bodies["body"].CollisionGroup = (short)CollisionGroup.AGENT;

            bodies["body"].UserData = this;

            if (!solid)
                bodies["body"].IsSensor = true;

            FixtureFactory.AttachRectangle(1f, 1f, 1f, Vector2.Zero, bodies["hotspot"]);
            bodies["hotspot"].FixedRotation = true;
            bodies["hotspot"].LinearVelocity = Vector2.Zero;
            bodies["hotspot"].BodyType = BodyType.Dynamic;

            

        }

        protected virtual void SetupSpriterPlayer()
        {
            sPlayer = new SpriterPlayer(factory._spriterManager.spriters["cursor"].getSpriterData(), 0, factory._spriterManager.loaders["loader1"]);
            sPlayer.setAnimation("hover", 0, 0);
            sPlayer.setFrameSpeed(20);
        }



        public void activateNavigator(Map m)
        {
            navigator = new Navigator(m, (int)size.X);
        }

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateStagger(gameTime);
            UpdateTimers(gameTime);
            UpdateTriggers(gameTime);

            ApplyStoppingFriction(gameTime);

            if (sm != null)
            {
                sm.Update(gameTime, this);
            }
            
            HandleCollisions();

            if(bodies.ContainsKey("hotspot"))
                bodies["hotspot"].Position = bodies["body"].WorldCenter;

            if(bodies.ContainsKey("activeArea"))
                bodies["activeArea"].Position = bodies["body"].WorldCenter;

            if (stats.get("hp") <= 0 && immortal == false)
                Remove();

            if (target != null && target.dead)
                target = null;
        }

        private void UpdateTimers(GameTime gameTime)
        {
            foreach (KeyValuePair<String, Timer> t in timers)
            {
                t.Value.Update(gameTime);
            }
        }

        private void UpdateTriggers(GameTime gameTime)
        {
            foreach (KeyValuePair<String, Trigger> t in triggers)
            {
                t.Value.Update(gameTime);
            }
        }

        private void ApplyStoppingFriction(GameTime gameTime)
        {
            bodies["body"].LinearVelocity /= (1f + stoppingFriction) * ((float)gameTime.ElapsedGameTime.Milliseconds / 16f); //is this ok???
        }

        private void UpdateStagger(GameTime gameTime)
        {
            if (stats.get("stagger") > 0)
                stats.addTo("stagger", -1); //change this someday to incorporate gameTime
            if (stats.get("stagger") <= 0)
            {
                stats.set("stagger", 0);
                staggered = false;
            }
            if (stats.get("stagger") >= stats.get("staggerLimit"))
            {
                staggered = true;
                stats.set("stagger", stats.get("staggerDuration"));
            }
        }
        

        protected void FollowPath(bool strafe)
        {
            if (path == null || path.Count == 0)
                return;

            if (path.Peek().box.Contains(new Point((int)(pos.X), (int)(pos.Y))))
            {
                path.Pop();
            }

            if (path.Count > 0)
            {
                Vector2 v = bodies["body"].LinearVelocity;

                if (path.Peek().pos.X + path.Peek().gridSize <= pos.X)
                    v.X += -speed;
                else if (pos.X < path.Peek().pos.X)
                    v.X += speed;

                if (pos.Y < path.Peek().pos.Y)
                    v.Y += speed;
                else if (path.Peek().pos.Y + path.Peek().gridSize <= pos.Y)
                    v.Y += -speed;

                if(!strafe)
                    facing = (Direction)
                        Helper.degreeToDirection((float)Math.Atan2(v.Y, v.X));

                bodies["body"].LinearVelocity = v;
            }

            
        }
        #endregion

        public void setTarget(Entity target) //public for testing
        {
            this.target = target;
            createPathToTarget();
        }

        public int distanceToEntity(Entity e)
        {
            return (int)Math.Sqrt(Math.Pow((pos.X + center.X) - (e.pos.X + e.center.X), 2)
                + Math.Pow((pos.Y + center.Y) - (e.pos.Y + e.center.Y), 2));
        }

        public int distanceToTarget()
        {
            if (target == null)
                return -1;

            return (int)Math.Sqrt(Math.Pow((pos.X + center.X) - (target.pos.X + target.center.X), 2)
                + Math.Pow((pos.Y + center.Y) - (target.pos.Y + target.center.Y), 2));
            /*return (int)(
                Math.Min(
                    Math.Abs(this.pos.X - target.pos.X), 
                    Math.Abs(this.pos.X + this.size.X - target.pos.X)) 
                + Math.Min(
                    Math.Abs(this.pos.Y - target.pos.Y),
                    Math.Abs(this.pos.Y + this.size.Y - target.pos.Y)));*/
        }

        protected void createPathToTarget()
        {
            if (target != null)
            {
                Point start = new Point((int)Math.Floor((pos.X + 1) / 10f), (int)Math.Floor((pos.Y + 1) / 10f)); //convert pos to gridPos for start
                Point goal = new Point((int)((target.pos.X+1) / 10), (int)(target.pos.Y+1) / 10); //convert target pos to gridPos for goal //hard coded in 10 for navigator gridSize (half of wall layer gridSize, matches object layer)
                path = navigator.getPath(start, goal);
            }
        }

        protected void createPathToLocation(Point goal)
        {
            Point start = new Point((int)(pos.X / 10f), (int)(pos.Y / 10f)); //convert pos to gridPos for start
            path = navigator.getPath(start, goal);
        }

        protected void createPathToLocation(Vector2 loc)
        {
            Point start = new Point((int)(pos.X / 10f), (int)(pos.Y / 10f)); //convert pos to gridPos for start
            Point goal = new Point((int)(loc.X / 10f), (int)(loc.Y / 10f)); //convert target pos to gridPos for goal //hard coded in 10 for navigator gridSize (half of wall layer gridSize, matches object layer)
            path = navigator.getPath(start, goal);
        }

        protected void attackForward(Vector2 attackSize, int damage, int force)
        {
            factory.addMeleeAttack(
                    new Vector2(
                        (float)(pos.X + center.X + size.X * Math.Cos(MathHelper.ToRadians((float)facing))), //x component of pos
                        (float)(pos.Y + center.Y + size.Y * Math.Sin(MathHelper.ToRadians((float)facing)))), //y component of pos
                    attackSize,
                    new Vector2(attackSize.X / 2, attackSize.Y / 2), //center point, useless i think, idk why i bother setting it here, Vector2.Zero could be memory saving
                    this,
                    damage,
                    force); //set parent to self, don't hurt self
        }

        protected virtual void HandleCollisions()
        {
            var c = bodies["hotspot"].ContactList;
            while(c != null)
            {
                if (c.Contact.IsTouching && c.Other.UserData is LayerType && ((LayerType)c.Other.UserData).Equals(LayerType.DEATHSOUP) && !airborne)
                    stats.set("hp", 0);

                c = c.Next;
            }
        }

        public override void FreeDraw()
        {
            //_spriterManager.DrawPlayer(sPlayer);
        }

        public override void Draw()
        {
            //sPlayer.update(bodies["body"].Position.X - _spriterManager._gameplay.camera.cameraPosition.X, bodies["body"].Position.Y - _spriterManager._gameplay.camera.cameraPosition.Y);
            //sPlayer.update(1440, -1080);
            sPlayer.update((bodies["body"].Position.X + center.X - factory._gm.camera.cameraPosition.X) * 2.25f,
                (bodies["body"].Position.Y + center.Y - factory._gm.camera.cameraPosition.Y) * -2.25f * (float)(Math.Sqrt(2)/2));
            _spriterManager.DrawPlayer(sPlayer);
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {

            base.DebugDraw(wTex, aTex, gd, spriteBatch, f, c, camera);

            //_spriterManager.DrawPlayer(sPlayer);

            if(visible)
                DebugDrawHealth(wTex, aTex, gd, spriteBatch, f, c, camera);
        }

        protected void DebugDrawHealth(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {
            int barWidth = 25;

            spriteBatch.Draw(wTex, new Rectangle(
                (int)pos.X - (int)camera.cameraPosition.X, 
                (int)pos.Y - 6 - (int)camera.cameraPosition.Y, 
                barWidth * stats.get("hp") / stats.get("maxHP"), 2), 
                Color.Red);
            if (staggered)
            {
                spriteBatch.Draw(wTex, new Rectangle(
                    (int)pos.X - (int)camera.cameraPosition.X, 
                    (int)pos.Y - 4 - (int)camera.cameraPosition.Y, 
                    barWidth * stats.get("stagger") / stats.get("staggerDuration"), 2), 
                    Color.Yellow);
            }
            else
            {
                spriteBatch.Draw(wTex, new Rectangle(
                    (int)pos.X - (int)camera.cameraPosition.X, 
                    (int)pos.Y - 4 - (int)camera.cameraPosition.Y, 
                    barWidth * stats.get("stagger") / stats.get("staggerLimit"), 2), 
                    Color.Green);
            }
        }

        protected void faceTarget()
        {
            if(target!=null)
                facing = Helper.degreeToDirection(angleToEntity(target));
        }

        protected bool hasLineOfSightToAgent(Agent a)
        {
            bool sight = true;
            List<Fixture> fList = _w.RayCast(pos + center, a.pos + a.center);

            foreach (Fixture f in fList)
            {
                if (f.Body.UserData.Equals(LayerType.WALL))
                    sight = false;
            }

            return sight;
        }

        protected void setTargetToPlayer()
        {
            foreach (Entity e in proximity)
            {
                if (e is PlayerAgent)
                    target = e;
            }
        }

        protected void setTargetToClosestPlayer()
        {
            List<PlayerAgent> players = new List<PlayerAgent>();
            int lowestDistance;

            foreach (Entity e in proximity)
            {
                if (e is PlayerAgent)
                    players.Add((PlayerAgent)e);
            }

            if (players.Count == 0)
                return;

            lowestDistance = distanceToEntity(players[0]);

            foreach (PlayerAgent p in players)
            {
                if (lowestDistance > this.distanceToEntity(p))
                    lowestDistance = this.distanceToEntity(p);
            }

            foreach (PlayerAgent p in players)
            {
                if (lowestDistance == this.distanceToEntity(p))
                    target = p;
            }
        }

        public virtual void TakeHit(Attack attack)
        {
            int damage = attack.damage;
            if (staggered)
                damage *= 2;

            stats.addTo("hp", -damage);

            float staggerMultiply = 1f;

            if (staggered)
                staggerMultiply = 10f;
            else
                stats.addTo("stagger", attack.force);

            Vector2 v = bodies["body"].LinearVelocity;

            Vector2 unitVector = new Vector2(
                (float)Math.Cos(MathHelper.ToRadians((float)attack.facing)),
                (float)Math.Sin(MathHelper.ToRadians((float)attack.facing)));

            v.X += unitVector.X * (float)attack.force * staggerMultiply * (float)stats.get("staggerDistance");
            v.Y += unitVector.Y * (float)attack.force * staggerMultiply * (float)stats.get("staggerDistance");

            bodies["body"].LinearVelocity = v;

            
        }

        private void drawPath()
        {

        }

        protected void ResetAllTimers()
        {
            foreach(KeyValuePair<string, Timer> t in timers)
            {
                t.Value.reset();
            }
        }

        public void fireBolt()
        {
            factory.addBolt(
                new Vector2(
                        (float)(pos.X + center.X + size.X * Math.Cos(MathHelper.ToRadians((float)facing))), //x component of pos
                        (float)(pos.Y + center.Y + size.Y * Math.Sin(MathHelper.ToRadians((float)facing)))), //y component of pos
                new Vector2(10f, 10f),
                new Vector2(5f, 5f),
                this,
                1,
                1);
        }

        public void fireAimedProjectile(int aimDirection)
        {
            factory.addAimedProjectile(
                new Vector2(
                        (float)(pos.X + center.X + size.X * Math.Cos(MathHelper.ToRadians((float)facing))), //x component of pos
                        (float)(pos.Y + center.Y + size.Y * Math.Sin(MathHelper.ToRadians((float)facing)))), //y component of pos
                new Vector2(10f, 10f),
                new Vector2(5f, 5f),
                this,
                1,
                1,
                aimDirection);
        }

        public void fireFireball(int clusters, float ballSize, int damage, int force)
        {
            factory.addFireball(
                new Vector2(
                    (float)(pos.X + center.X + size.X * Math.Cos(MathHelper.ToRadians((float)facing))),
                    (float)(pos.Y + center.Y + size.Y * Math.Sin(MathHelper.ToRadians((float)facing)))),
                    new Vector2(ballSize, ballSize),
                    new Vector2(ballSize / 2f, ballSize / 2f),
                    this,
                    this.facing,
                    damage,
                    force,
                    clusters);
        }
    }
}
