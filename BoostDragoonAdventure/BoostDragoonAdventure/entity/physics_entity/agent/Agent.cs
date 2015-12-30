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
using wickedcrush.entity.physics_entity.agent.action;
using wickedcrush.particle;
using wickedcrush.entity.physics_entity.agent.trap.triggerable;
using wickedcrush.manager.gameplay;

namespace wickedcrush.entity.physics_entity.agent
{
    public enum StateName
    {
        Standing,
        Moving,
        Tell,
        Attacking,
        Staggered,
        CounterAttack,
        Jumping,
        Landing
    }

    public struct SpriterOffsetStruct
    {
        public SpriterPlayer player;
        public Vector2 offset;

        public SpriterOffsetStruct(SpriterPlayer player, Vector2 offset)
        {
            this.player = player;
            this.offset = offset;
        }
    }

    public class Agent : PhysicsEntity
    {
        protected Navigator navigator;
        protected Stack<PathNode> path;
        protected Dictionary<String, Timer> timers;
        protected Dictionary<String, Trigger> triggers;
        protected StateTree stateTree;
        public EntityFactory factory;
        protected SpriterManager _spriterManager;
        protected ParticleEmitter particleEmitter;

        public List<Entity> proximity;
        protected Entity target;
        protected Vector2 searchPosition = Vector2.Zero, initialPosition = Vector2.Zero;

        public PersistedStats stats;

        protected float speed = 50f, targetSpeed = 50f, boostSpeed = 120f;
        public float activeRange = 0f;
        //protected bool strafe = false;
        protected float driftDirection = 0f;
        public bool staggered = false;

        public Dictionary<String,SpriterPlayer> sPlayers;

        public SpriterPlayer bodySpriter, overlaySpriter, shadowSpriter;
        public Dictionary<String, SpriterOffsetStruct> hudSpriters;

        public bool drawShadow = false;

        public bool hitThisTick = false;

        protected Random random = new Random();


        //item stuff
        public bool pressReady = true, holdReady = false, releaseReady = false, overheating = false;

        //public bool itemInPress = false;
        //public bool itemInHold = false;
        //public bool itemInUse = false;

        public float _depth = 0f;

        const float poopNear = -0.236f, poopFar = 0.06199996f;

        public float spriteScaleAmount = 70f;

        protected KeyValuePair<Color, Timer> flashColor = new KeyValuePair<Color, Timer>(Color.Red, new Timer(240));


        public bool targetable = false;

        public int staggerHeight = 100;
        public int flightHeight = 100;

        public bool inJump = false;
        public Vector2 jumpDestination = Vector2.Zero;
        public Vector2 jumpStartPoint = Vector2.Zero;

        public int physicalDMG = 0, etheralDMG = 0;
        //public int jumpHeight = 0;

        public Agent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, SoundManager sound)
            : base(w, pos, size, center, solid, sound)
        {
            initialPosition = pos;
            _spriterManager = factory._spriterManager;
            Initialize(new PersistedStats(5, 5), factory);
        }

        public Agent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, PersistedStats stats, SoundManager sound)
            : base(w, pos, size, center, solid, sound)
        {
            initialPosition = pos;
            _spriterManager = factory._spriterManager;
            Initialize(stats, factory);
        }

        private void Initialize(PersistedStats stats, EntityFactory factory)
        {
            this.factory = factory;
            this.stats = stats;

            //stats.set("staggerDistance", 0);

            timers = new Dictionary<String, Timer>();
            triggers = new Dictionary<String, Trigger>();
            proximity = new List<Entity>();
            hudSpriters = new Dictionary<String, SpriterOffsetStruct>();
            this.name = "Agent";

            this.particleEmitter = new ParticleEmitter(factory._particleManager);

            SetupSpriterPlayer();

            timers.Add("falling", new Timer(500));

            skillHeight = 15;
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

        }

        public void TriggerInProximity()
        {
            foreach (Entity e in proximity)
            {
                if (e is Triggerable)
                {
                    //((Triggerable)e).activate();
                    ((Triggerable)e).delayedActivate(this.distanceToEntity(e) * 5);
                }
            }
        }

        protected virtual void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("cursor", new SpriterPlayer(factory._spriterManager.spriters["all"].getSpriterData(), 2, factory._spriterManager.spriters["all"].loader));
            sPlayers.Add("hud", new SpriterPlayer(factory._spriterManager.spriters["all"].getSpriterData(), 4, factory._spriterManager.spriters["all"].loader));
            

            bodySpriter = sPlayers["cursor"];
            bodySpriter.setAnimation("hover", 0, 0);
            bodySpriter.setFrameSpeed(20);

            sPlayers.Add("shadow", new SpriterPlayer(factory._spriterManager.spriters["shadow"].getSpriterData(), 0, factory._spriterManager.spriters["shadow"].loader));
            shadowSpriter = sPlayers["shadow"];
            shadowSpriter.setAnimation("still", 0, 0);
            shadowSpriter.setFrameSpeed(20);
            drawShadow = true;
        }

        public void AddHudElement(string key, string elementName, int entityId, Vector2 offset) //entity id in spriter file
        {
            int hud = entityId;
            SpriterOffsetStruct temp = new SpriterOffsetStruct(
                new SpriterPlayer(factory._spriterManager.spriters["all"].getSpriterData(), hud, factory._spriterManager.spriters["all"].loader), 
                offset);
            temp.player.setAnimation(elementName, 0, 0);
            if (!hudSpriters.ContainsKey(key))
                hudSpriters.Add(key, temp);
            else
                hudSpriters[key] = temp;
        }

        public void ScaleOverheadWeapon(String key, float scale)
        {
            if (hudSpriters.ContainsKey(key))
                hudSpriters[key].player.setScale(scale);
        }

        public void AddOverheadWeapon(String key, String spriterName, String animationName, int entityIndex, Vector2 offset, float scale, float angle)
        {
            SpriterOffsetStruct temp = new SpriterOffsetStruct(
                new SpriterPlayer(factory._spriterManager.spriters[spriterName].getSpriterData(), entityIndex, factory._spriterManager.spriters[spriterName].loader),
                offset);
            temp.player.setAnimation(animationName, 0, 0);
            temp.player.setAngle(angle);
            temp.player.setScale(scale);

            if (!hudSpriters.ContainsKey(key))
                hudSpriters.Add(key, temp);
            else
                hudSpriters[key] = temp;
        }

        public void RemoveOverheadWeapon(string key)
        {
            if(hudSpriters.ContainsKey(key))
                hudSpriters.Remove(key);
        }

        public void RemoveHudElement(string key)
        {
            if(hudSpriters.ContainsKey(key))
                hudSpriters.Remove(key);
        }



        public void activateNavigator(Map m)
        {
            navigator = new Navigator(m, (int)size.X);
        }

        #region Update
        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);

            //speed = ((speed + targetSpeed) * 29f) / 30f;
            speed = targetSpeed;

            hitThisTick = false;

            UpdateStagger(gameTime);
            UpdateTimers(gameTime);
            UpdateTriggers(gameTime);

            ApplyStoppingFriction(gameTime);

            if (stateTree != null)
            {
                stateTree.Update(gameTime, this);
            }

            //if (!inJump)
            //{
                HandleCollisions();
            //}
            if(inJump)
            {
                if (timers["jump"].isDone())
                {
                    EndJump();
                } 
                else
                {
                    this.SetPosNoCenter(new Vector2(
                        MathHelper.Lerp(jumpStartPoint.X, jumpDestination.X, timers["jump"].getPercent()),
                        MathHelper.Lerp(jumpStartPoint.Y, jumpDestination.Y, timers["jump"].getPercent())));

                    this.height = (int)(Math.Sin(timers["jump"].getPercent() * Math.PI) * flightHeight);

                    if (height > 20)
                        noCollision = true;
                }


            }

            if (stats.get("hp") <= 0 && immortal == false)
            {
                Die();
                //return;
            }

            if (target != null && target.dead)
                target = null;

            facing = Helper.constrainDirection(facing);

            UpdateSpriters();

            skillHeight = height + 15;

        }

        public void EmitParticles(ParticleStruct ps, int count)
        {
            particleEmitter.EmitParticles(ps, factory, count);
        }

        protected virtual void Die()
        {


            if (!timers["falling"].isActive() || timers["falling"].isDone())
            {
                //this.remove = true;
                Remove();
                PlayCue("horrible death");


                ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, this.height, this.pos.Y + this.center.Y), Vector3.Zero, new Vector3(-0.5f, 3f, -0.5f), new Vector3(1f, 1f, 1f), new Vector3(0, -.1f, 0), 0f, 0f, 1000, "all", 3, "hit", 0.25f);
                particleEmitter.EmitParticles(ps, factory, 10);
                ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, this.height, this.pos.Y + this.center.Y), Vector3.Zero, new Vector3(0f, 5f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0, -.1f, 0), 0f, 0f, 1000, "all", 3, "hit", 0.25f);
                particleEmitter.EmitParticles(ps, factory, 1);
            }
        }

        private void UpdateTimers(GameTime gameTime)
        {
            foreach (KeyValuePair<String, Timer> t in timers)
            {
                t.Value.Update(gameTime);
            }

            flashColor.Value.Update(gameTime);
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
            //bodies["body"].LinearVelocity /= (1f + stoppingFriction) * ((float)gameTime.ElapsedGameTime.Milliseconds / 17f); //is this ok???
            bodies["body"].LinearVelocity /= (1f + stoppingFriction) * 2f * ((float)gameTime.ElapsedGameTime.Milliseconds / 17f);
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
                factory.addText("Staggered!", this.pos, 1000);
                staggered = true;
                stats.set("stagger", stats.get("staggerDuration"));
                stats.set("charge", 0);
            }
        }

        protected void MoveForward(bool strafe, float amount)
        {
            Vector2 v = bodies["body"].LinearVelocity;

            v += new Vector2((float)(amount * Math.Cos(MathHelper.ToRadians((float)movementDirection)) * (elapsed.TotalMilliseconds / 17.0)),
                        (float)(amount * Math.Sin(MathHelper.ToRadians((float)movementDirection)) * (elapsed.TotalMilliseconds / 17.0)));

            //v += new Vector2((float)(amount * Math.Cos(MathHelper.ToRadians((float)movementDirection)) ),
              //          (float)(amount * Math.Sin(MathHelper.ToRadians((float)movementDirection)) ));

            if (!strafe)
                facing = (Direction)
                    Helper.radiansToDirection(MathHelper.ToRadians((float)movementDirection));

            bodies["body"].LinearVelocity = v;
        }

        protected void BoostForward()
        {
            Vector2 v = bodies["body"].LinearVelocity;

            driftDirection = (float)aimDirection;

            Vector2 unitVector = new Vector2(
                (float)Math.Cos(MathHelper.ToRadians(driftDirection)),
                (float)Math.Sin(MathHelper.ToRadians(driftDirection))
            );



            v += unitVector * boostSpeed;

            bodies["body"].LinearVelocity += v;

            airborne = true;
        }

        protected void BoostBackward()
        {
            Vector2 v = bodies["body"].LinearVelocity;

            driftDirection = (float)aimDirection + 180;

            Vector2 unitVector = new Vector2(
                (float)Math.Cos(MathHelper.ToRadians(driftDirection)),
                (float)Math.Sin(MathHelper.ToRadians(driftDirection))
            );

            v += unitVector * boostSpeed;

            bodies["body"].LinearVelocity += v;

            airborne = true;
        }

        protected void Strafe(float amount)
        {
            Vector2 v = bodies["body"].LinearVelocity;

            v += new Vector2((float)(amount * Math.Sin(MathHelper.ToRadians((float)movementDirection)) * (elapsed.TotalMilliseconds / 17.0)),
                        (float)(-amount * Math.Cos(MathHelper.ToRadians((float)movementDirection)) * (elapsed.TotalMilliseconds / 17.0)));

            bodies["body"].LinearVelocity = v;
        }



        protected void FollowPath(bool strafe)
        {
            if (path == null || path.Count == 0)
                return;

            /*if (path.Peek().box.Intersects(new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y)))
            {
                path.Pop();
            }*/

            if (path.Peek().box.Contains(new Point((int)pos.X, (int)pos.Y)))
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
                        Helper.radiansToDirection((float)Math.Atan2(v.Y, v.X));

                bodies["body"].LinearVelocity = v;

                /*facePosition(path.Peek().pos + new Vector2(5, 5));
                MoveForward(strafe, speed);*/
            }

            
        }
        #endregion

        public void setTarget(Entity target) //public for testing
        {
            this.target = target;
            createPathToTarget();
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

        

        public int distanceToSearchPos()
        {
            return (int)Math.Sqrt(Math.Pow((pos.X + center.X) - (searchPosition.X), 2)
                + Math.Pow((pos.Y + center.Y) - (searchPosition.Y), 2));
        }

        protected void createPathToTarget()
        {
            if (target != null)
            {
                Point start = new Point((int)Math.Floor((pos.X + 1) / 10f), (int)Math.Floor((pos.Y + 1) / 10f)); //convert pos to gridPos for start
                Point goal = new Point((int)Math.Floor((target.pos.X + 0f) / 10f), (int)Math.Floor((target.pos.Y + 0f) / 10f)); //convert target pos to gridPos for goal //hard coded in 10 for navigator gridSize (half of wall layer gridSize, matches object layer)
                path = navigator.getPath(start, goal);
                //optimizePath();
            }
        }

        protected void optimizePath()
        {
            if (path == null)
                return;
            
            while (path.Count > 1)
            {
                if (pathToLocationIsClear(path.ElementAt<PathNode>(1).pos))
                {
                    path.Pop();
                }
                else
                {
                    return;
                }
            }
            
        }

        protected void createPathToLocation(Point goal)
        {
            Point start = new Point((int)(pos.X / 10f), (int)(pos.Y / 10f)); //convert pos to gridPos for start
            path = navigator.getPath(start, goal);
            //optimizePath();
        }

        protected void createPathToLocation(Vector2 loc)
        {
            Point start = new Point((int)Math.Floor((pos.X + 1) / 10f), (int)Math.Floor((pos.Y + 1) / 10f)); //convert pos to gridPos for start
            Point goal = new Point((int)Math.Floor(loc.X / 10f), (int)Math.Floor(loc.Y / 10f)); //convert target pos to gridPos for goal //hard coded in 10 for navigator gridSize (half of wall layer gridSize, matches object layer)
            path = navigator.getPath(start, goal);
            //optimizePath();
        }

        protected bool pathToLocationIsClear(Vector2 loc)
        {
            bool sight = true;
            List<Fixture> fList = _w.RayCast(pos + center, loc);
            foreach (Fixture f in fList)
            {
                if (f.Body.UserData.Equals(LayerType.WALL) || f.Body.UserData.Equals(LayerType.DEATHSOUP))
                    sight = false;
            }


            return sight;
        }

        protected bool pathBetweenPointsIsClear(Vector2 a, Vector2 b)
        {
            bool sight = true;
            List<Fixture> fList = _w.RayCast(a, b);

            foreach (Fixture f in fList)
            {
                if (f.Body.UserData.Equals(LayerType.WALL) || f.Body.UserData.Equals(LayerType.DEATHSOUP))
                    sight = false;
            }

            return sight;
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
            

            if (stats.get("hp") > 0 && factory._gm.map.getLayer(LayerType.DEATHSOUP).collision(new Rectangle((int)(this.pos.X + this.center.X - 1), (int)(this.pos.Y + this.center.Y - 1), 2, 2)) && !this.airborne)
            {
                stats.set("hp", 0);
                timers["falling"].resetAndStart();
            }
        }

        public override void FreeDraw()
        {
            //_spriterManager.DrawPlayer(sPlayer);
        }

        protected void UpdateSpriters()
        {
            if (visible && bodies.ContainsKey("body"))
            {

                float shift = 1.5f;

                Vector2 spritePos = new Vector2(
                    (bodies["body"].Position.X + center.X - factory._gm.camera.cameraPosition.X) * (2f / factory._gm.camera.zoom) * 2.25f - 500 * (2f - factory._gm.camera.zoom),
                    ((bodies["body"].Position.Y + (center.Y * shift) - factory._gm.camera.cameraPosition.Y - height) * (2f / factory._gm.camera.zoom) * -2.25f * (float)(Math.Sqrt(2) / 2) + 240 * (2f - factory._gm.camera.zoom) - 100)
                    );


                float temp = ((bodies["body"].Position.Y + (center.Y * shift) - factory._gm.camera.cameraPosition.Y) * (2f / factory._gm.camera.zoom) * -2.25f * (float)(Math.Sqrt(2) / 2) + 240 * (2f - factory._gm.camera.zoom) - 100);
                
                float depth = MathHelper.Lerp(0.97f + poopNear, 0.37f + poopFar, temp / -1080f) - 0.00f; //so bad

                /*if (factory._gm._playerManager.getPlayerList()[0].c.KeyPressed(Microsoft.Xna.Framework.Input.Keys.U)) {
                    poopNear += 0.001f;
                    Console.Out.WriteLine("near: " + poopNear);
                }

                if (factory._gm._playerManager.getPlayerList()[0].c.KeyPressed(Microsoft.Xna.Framework.Input.Keys.I)) {
                    poopNear -= 0.001f;
                    Console.Out.WriteLine("near: " + poopNear);
                }

                if (factory._gm._playerManager.getPlayerList()[0].c.KeyPressed(Microsoft.Xna.Framework.Input.Keys.O))
                {
                    poopFar += 0.001f;
                    Console.Out.WriteLine("far: " + poopFar);
                }

                if (factory._gm._playerManager.getPlayerList()[0].c.KeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
                {
                    poopFar -= 0.001f;
                    Console.Out.WriteLine("far: " + poopFar);
                }*/


                //old way: -10 to 10
                //bodySpriter.setScale((((float)size.X) / 10f) * (2f / factory._gm.camera.zoom));
                //

                bodySpriter.setScale((((float)size.X) / spriteScaleAmount) * (2f / factory._gm.camera.zoom));

                _depth = depth;
                bodySpriter.SetDepth(depth);

                bodySpriter.update(spritePos.X,
                        spritePos.Y);

                if (null != overlaySpriter)
                {
                    overlaySpriter.setScale((((float)size.X) / spriteScaleAmount) * (2f / factory._gm.camera.zoom));
                    overlaySpriter.SetDepth(depth - 0.009f);
                    overlaySpriter.update(spritePos.X, spritePos.Y);
                }

                if (null != shadowSpriter && drawShadow)
                {
                    shadowSpriter.setScale((((float)size.X) / (spriteScaleAmount / 10f)) * (2f / factory._gm.camera.zoom));
                    shadowSpriter.SetDepth(depth + 0.001f);

                    Vector2 shadowPos = new Vector2(
                        (bodies["body"].Position.X + center.X - factory._gm.camera.cameraPosition.X) * (2f / factory._gm.camera.zoom) * 2.25f - 500 * (2f - factory._gm.camera.zoom),
                        ((bodies["body"].Position.Y + (center.Y) - factory._gm.camera.cameraPosition.Y) * (2f / factory._gm.camera.zoom) * -2.25f * (float)(Math.Sqrt(2) / 2) + 240 * (2f - factory._gm.camera.zoom) - 100)
                    );

                    shadowSpriter.update(shadowPos.X, shadowPos.Y);
                }


                foreach (KeyValuePair<String, SpriterOffsetStruct> s in hudSpriters)
                {
                    s.Value.player.SetDepth(_depth-0.03f);
                    s.Value.player.update(spritePos.X + s.Value.offset.X,
                        (spritePos.Y + s.Value.offset.Y));
                }
            }
        }

        public override void Draw(bool depthPass, Dictionary<string, PointLightStruct> lightList, GameplayManager gameplay)
        {
            if (flashColor.Value.isActive())
            {
                factory._gm._screen.litSpriteEffect.Parameters["solidColor"].SetValue(new Vector4(flashColor.Key.R, flashColor.Key.G, flashColor.Key.B, 1));
            }
            else
            {
                factory._gm._screen.litSpriteEffect.Parameters["solidColor"].SetValue(new Vector4(0, 0, 0, 1));
            }

            base.Draw(depthPass, lightList, gameplay);
            if (visible && bodies.ContainsKey("body"))
            {

                factory._gm._screen.spriteEffect.Parameters["depth"].SetValue(bodySpriter.depth);
                if (depthPass)
                {
                    gameplay.scene.spriteEffect.CurrentTechnique.Passes["Depth"].Apply();
                    _spriterManager.DrawPlayer(bodySpriter);
                }
                else 
                {

                    gameplay._screen.litSpriteEffect.Parameters["AmbientColor"].SetValue(new Vector4(0.8f, 0.8f, 1f, 1f));
                    gameplay._screen.litSpriteEffect.Parameters["AmbientIntensity"].SetValue(0.015f);
                    gameplay._screen.litSpriteEffect.Parameters["baseColor"].SetValue(new Vector4(0.02f, 0.02f, 0.05f, 1f));

                    /*if (flashColor.Value.isActive())
                    {
                        //factory._gm._screen.litSpriteEffect.CurrentTechnique = factory._gm._screen.litSpriteEffect.Techniques["SolidColor"];
                        factory._gm._screen.litSpriteEffect.Parameters["solidColor"].SetValue(new Vector4(flashColor.Key.R, flashColor.Key.G, flashColor.Key.B, 1));
                        //gameplay._screen.litSpriteEffect.CurrentTechnique.Passes["SolidColor"].Apply();

                        //_spriterManager.DrawPlayer(bodySpriter);

                        //factory._gm._screen.litSpriteEffect.CurrentTechnique = factory._gm._screen.litSpriteEffect.Techniques["MultiPassLight"];
                    }
                    else
                    {
                        factory._gm._screen.litSpriteEffect.Parameters["solidColor"].SetValue(new Vector4(0, 0, 0, 1));
                        
                    }*/

                    gameplay._screen.litSpriteEffect.CurrentTechnique.Passes["Unlit"].Apply();
                    _spriterManager.DrawPlayer(bodySpriter);
                    
                }

                
                

                if (null != overlaySpriter)
                {

                    factory._gm._screen.spriteEffect.Parameters["depth"].SetValue(overlaySpriter.depth);
                    _spriterManager.DrawPlayer(overlaySpriter); // todo: depth offset

                }

                if (null != shadowSpriter && drawShadow)
                {

                    if (!depthPass)
                    {
                        gameplay._screen.litSpriteEffect.CurrentTechnique.Passes["Unlit"].Apply();
                        factory._gm._screen.spriteEffect.Parameters["depth"].SetValue(shadowSpriter.depth);
                        _spriterManager.DrawPlayer(shadowSpriter); // todo: depth offset
                    }
                }


                foreach (KeyValuePair<String, SpriterOffsetStruct> s in hudSpriters)
                {
                    gameplay._screen.litSpriteEffect.CurrentTechnique.Passes["Unlit"].Apply();
                    factory._gm._screen.spriteEffect.Parameters["depth"].SetValue(s.Value.player.depth);
                    _spriterManager.DrawPlayer(s.Value.player);
                }
            }


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
                barWidth * stats.get("hp") / stats.get("MaxHP"), 2), 
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
            if (target != null)
            {
                //facing = Helper.radiansToDirection(angleToEntity(target));
                //aimDirection = (int)MathHelper.ToDegrees(angleToEntity(target));
                //movementDirection = (int)MathHelper.ToDegrees(angleToEntity(target));
                faceEntity(target);
            }
        }

        protected void faceEntity(Entity e, int degreeChange)
        {
            if (e == null)
                return;

            facing = Helper.radiansToDirection(angleToEntity(e) + MathHelper.ToRadians(degreeChange));
            aimDirection = (int)Helper.ConstrainDegrees(MathHelper.ToDegrees(angleToEntity(e)) + degreeChange);
            movementDirection = (int)Helper.ConstrainDegrees(MathHelper.ToDegrees(angleToEntity(e)) + degreeChange);
        }

        protected void faceEntity(Entity e)
        {
            faceEntity(e, 0);
        }

        protected void facePosition(Vector2 pos)
        {

            movementDirection = (int)MathHelper.ToDegrees(directionVectorToAngle(vectorToPos(pos)));
            aimDirection = (int)MathHelper.ToDegrees(directionVectorToAngle(vectorToPos(pos)));
        }

        protected bool hasLineOfSightToEntity(Entity e)
        {
            if (e == null)
                return false;

            bool sight = true;
            List<Fixture> fList = _w.RayCast(pos + center, e.pos + e.center);

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

        protected void setTargetToClosestPlayer(bool lineOfSight, int fov)
        {
            target = null;
            List<PlayerAgent> players = new List<PlayerAgent>();
            int lowestDistance;
            
            foreach (Entity e in proximity)
            {
                if (e is PlayerAgent)
                {
                    if (lineOfSight)
                    {
                        if(hasLineOfSightToEntity(e) && Helper.angleInFov((int)facing, (int)Helper.radiansToDirection(angleToEntity(e)), fov))
                            players.Add((PlayerAgent)e);
                    }
                    else
                    {
                        players.Add((PlayerAgent)e);
                    }
                }
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

        public virtual void TakeSkill(ActionSkill action)
        {
            if (this.immortal || remove || hitThisTick)
                return;

            hitThisTick = true;

            foreach(KeyValuePair<string, int> pair in action.statIncrement)
            {
                int amount = pair.Value;

                if (pair.Key.Equals("hp"))
                {
                    if (this.staggered)
                    {
                        amount *= 2;
                    }
                    if (this is PlayerAgent)
                        factory.addText(amount.ToString(), pos + new Vector2((float)(random.NextDouble() * 50), (float)(random.NextDouble() * 50)), 1000, Color.Red);
                    else
                        factory.addText(amount.ToString(), pos + new Vector2((float)(random.NextDouble() * 50), (float)(random.NextDouble() * 50)), 1000);
                }

                if (stats.numbersContainsKey(pair.Key))
                {
                    stats.addTo(pair.Key, amount);
                }
                else
                {
                    stats.set(pair.Key, amount);
                }

                
            }

            ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, this.height, this.pos.Y + this.center.Y), Vector3.Zero, new Vector3(-1.5f, 3f, -1.5f), new Vector3(3f, 3f, 3f), new Vector3(0, -.3f, 0), 0f, 0f, 2000, "all", 3, "hit", 0.25f);
            //particleEmitter.EmitParticles(ps, this.factory, 3);

            Vector2 bloodspurt = new Vector2((float)(Math.Cos(MathHelper.ToRadians((float)action.facing)) + 0f * Math.Sin(MathHelper.ToRadians((float)action.facing))),
                (float)(Math.Sin(MathHelper.ToRadians((float)action.facing)) - 0f * Math.Cos(MathHelper.ToRadians((float)action.facing))));

            ps = new ParticleStruct(new Vector3(action.pos.X + action.center.X, action.height + 10, action.pos.Y + action.center.Y), Vector3.Zero, new Vector3(bloodspurt.X * 3f, 2f, bloodspurt.Y * 3f), new Vector3(1.5f, 4f, -1.5f), new Vector3(0, -.3f, 0), 0f, 0f, 500, "particles", 3, "bloodspurt_001");
            particleEmitter.EmitParticles(ps, this.factory, 5);

            Vector2 v = bodies["body"].LinearVelocity;

            Vector2 unitVector = new Vector2(
                (float)Math.Cos(MathHelper.ToRadians((float)action.force.Key)),
                (float)Math.Sin(MathHelper.ToRadians((float)action.force.Key)));

            if(!staggered)
                stats.addTo("stagger", action.force.Value);

            v.X += unitVector.X * (float)action.force.Value * 30f * (float)stats.get("staggerDistance");
            v.Y += unitVector.Y * (float)action.force.Value * 30f * (float)stats.get("staggerDistance");

            if (bodies.ContainsKey("body"))
                bodies["body"].LinearVelocity = v;

            action.PlayTakeSound();

            factory._gm.camera.ShakeScreen(5f);

            //factory._gm.activateFreezeFrame();
        }

        public virtual void TakeHit(Attack attack)
        {
            TakeHit(attack, false);
        }

        public virtual void TakeKnockback(Vector2 pos, float force)
        {
            Vector2 unitVector = vectorToPos(pos);

            Vector2 v = new Vector2( 
                unitVector.X * force * 1000f * (float)stats.get("staggerDistance"),
                unitVector.Y * force * 1000f * (float)stats.get("staggerDistance"));

            if (bodies.ContainsKey("body"))
                bodies["body"].LinearVelocity = v;

            _sound.playCue("hurt", emitter);

            ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, this.height + 20, this.pos.Y + this.center.Y), Vector3.Zero, new Vector3(-1.5f, 3f, -1.5f), new Vector3(3f, 3f, 3f), new Vector3(0, -.3f, 0), 0f, 0f, 500, "particles", 3, "bloodspurt_001");
            particleEmitter.EmitParticles(ps, this.factory, 3);

            factory._gm.camera.ShakeScreen(5f);

            //factory._gm.activateFreezeFrame();
        }

        public virtual void TakeKnockback(Vector2 pos, int dmg, float force)
        {
            if (this.immortal || remove || hitThisTick)
                return;

            int damage = dmg;
            float staggerMultiply = 1f;

            hitThisTick = true;

            if (staggered)
                damage *= 2;

            stats.addTo("hp", -damage);

            if (staggered)
                staggerMultiply = 10f;
            else
                stats.addTo("stagger", (int)force);

            Vector2 unitVector = vectorToPos(pos);

            Vector2 v = new Vector2(
                unitVector.X * force * 1000f * (float)stats.get("staggerDistance"),
                unitVector.Y * force * 1000f * (float)stats.get("staggerDistance"));

            if (bodies.ContainsKey("body"))
                bodies["body"].LinearVelocity = v;

            _sound.playCue("hurt", emitter);

            ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, this.height, this.pos.Y + this.center.Y), Vector3.Zero, new Vector3(-1.5f, 3f, -1.5f), new Vector3(3f, 3f, 3f), new Vector3(0, -.3f, 0), 0f, 0f, 2000, "all", 3, "hit", 0.25f);
            //particleEmitter.EmitParticles(ps, this.factory, 3);

            Vector2 bloodspurt = new Vector2((float)(Math.Cos(MathHelper.ToRadians((float)this.facing)) + 0f * Math.Sin(MathHelper.ToRadians((float)this.facing))),
                (float)(Math.Sin(MathHelper.ToRadians((float)this.facing)) - 0f * Math.Cos(MathHelper.ToRadians((float)this.facing))));

            ps = new ParticleStruct(new Vector3(pos.X, this.height + 10, pos.Y), Vector3.Zero, new Vector3(bloodspurt.X * 3f, 2f, bloodspurt.Y * 3f), new Vector3(1.5f, 4f, -1.5f), new Vector3(0, -.3f, 0), 0f, 0f, 500, "particles", 3, "bloodspurt_001");
            particleEmitter.EmitParticles(ps, this.factory, 5);

            factory._gm.camera.ShakeScreen(5f);
            factory._gm.activateFreezeFrame();

            factory.addText("-" + damage.ToString(), pos + new Vector2((float)(random.NextDouble() * 50), (float)(random.NextDouble() * 50)), 1000);
            
        }

        public virtual void TakeHit(Attack attack, bool knockback)
        {
            if (this.immortal || remove || hitThisTick)
                return;

            hitThisTick = true;

            Vector2 unitVector;

            int damage = attack.damage;
            if (staggered)
                damage *= 2;

            stats.addTo("hp", -damage);

            float staggerMultiply = 1f;

            if (staggered)
                staggerMultiply = 10f;
            else
                stats.addTo("stagger", attack.force);

            Vector2 v = new Vector2();

            if (knockback)
            {
                unitVector = vectorToEntity(attack);
            }
            else
            {
                unitVector = new Vector2(
                    (float)Math.Cos(MathHelper.ToRadians((float)attack.facing)),
                    (float)Math.Sin(MathHelper.ToRadians((float)attack.facing)));
            }

            
            v.X = unitVector.X * (float)attack.force * 1000f * staggerMultiply * (float)stats.get("staggerDistance");
            v.Y = unitVector.Y * (float)attack.force * 1000f * staggerMultiply * (float)stats.get("staggerDistance");

            if(bodies.ContainsKey("body"))
                bodies["body"].LinearVelocity = v;

            factory.addText("-" + damage.ToString(), pos + new Vector2((float)(random.NextDouble() * 50), (float)(random.NextDouble() * 50)), 1000);
            _sound.playCue("hurt", emitter);

            ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, this.height + 20, this.pos.Y + this.center.Y), Vector3.Zero, new Vector3(-1.5f, 3f, -1.5f), new Vector3(3f, 3f, 3f), new Vector3(0, -.3f, 0), 0f, 0f, 500, "particles", 3, "bloodspurt_001");
            particleEmitter.EmitParticles(ps, this.factory, 3);

            factory._gm.camera.ShakeScreen(5f);

            //factory._gm.activateFreezeFrame();
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

        public void useActionSkill(SkillStruct skill)
        {
            factory.addActionSkill(skill, this);
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
                10,
                500);
        }

        public void fireAimedProjectile(int aimDirection)
        {
            factory.addAimedProjectile(
                new Vector2(
                        (float)(pos.X + center.X + size.X * Math.Cos(MathHelper.ToRadians((float)aimDirection))), //x component of pos
                        (float)(pos.Y + center.Y + size.Y * Math.Sin(MathHelper.ToRadians((float)aimDirection)))), //y component of pos
                new Vector2(10f, 10f),
                new Vector2(5f, 5f),
                this,
                10,
                500,
                aimDirection);
        }

        public void fireTrackingProjectile(int aimDirection, Entity target)
        {
            factory.addTrackingProjectile(
                new Vector2(
                        (float)(pos.X + center.X), //x component of pos
                        (float)(pos.Y + center.Y )), //y component of pos
                        new Vector2(30f, 30f),
                        10,
                        100,
                        this,
                        target,
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

        protected void InitializeHpBar()
        {
            AddHudElement("hp_bar", "hp_bar", 4, Vector2.Zero);
            //AddHudElement("fuel_bar", "fuel_bar", 4, Vector2.Zero);
        }

        protected void RemoveHpBar()
        {
            RemoveHudElement("hp_bar");
            //RemoveHudElement("fuel_bar");
        }

        protected void UpdateHpBar()
        {
            long fudgeSunday = 100 - (long)((((double)stats.get("hp")) / ((double)stats.get("MaxHP"))) * 99.0);
            //long fuelSunday = 100 - (long)((((double)stats.get("boost")) / ((double)stats.get("maxBoost"))) * 99.0);
            hudSpriters["hp_bar"].player.setFrame(fudgeSunday);
            //hudSpriters["fuel_bar"].player.setFrame(fuelSunday);



            //hudSpriters["hp_bar"].setFrame(1);
        }

        protected void StartJump(int jumpHeight, int ms, Vector2 destination)
        {
            
            this.flightHeight = jumpHeight;
            this.jumpStartPoint = this.pos;
            this.jumpDestination = destination - this.center;
            //this.bodies["body"].IsSensor = true;

            addTimer("jump", ms);
            timers["jump"].resetAndStart();
            airborne = true;
            inJump = true;
        }

        protected void EndJump()
        {
            this.height = 0;
            airborne = false;
            inJump = false;
            noCollision = false;
            //this.bodies["body"].IsSensor = false;
        }

        protected void BeginFlight(int height)
        {
            airborne = true;
            this.bodies["body"].IsSensor = true;
        }

        protected void EndFlight(int height)
        {
            airborne = false;
            this.bodies["body"].IsSensor = false;
        }

        public void addTimer(String key, int ms)
        {
            if(timers.ContainsKey(key))
            {
                timers[key].setInterval(ms);
                timers[key].reset();
            } else {
                timers.Add(key, new Timer(ms));
            }
        }

        public void FlashColor(Color color, int ms)
        {
            flashColor = new KeyValuePair<Color, Timer>(color, new Timer(ms));
            flashColor.Value.resetAndStart();
        }

        public void removeTimer(String key)
        {
            timers.Remove(key);
        }

        public Timer getTimer(String key)
        {
            if (timers.ContainsKey(key))
            {
                return timers[key];
            }

            throw new Exception("No timer exists!");
        }
    }
}
