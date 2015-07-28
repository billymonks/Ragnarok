using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.factory.entity;
using wickedcrush.utility;
using Microsoft.Xna.Framework;
using wickedcrush.stats;
using FarseerPhysics.Dynamics;
using wickedcrush.behavior;
using wickedcrush.behavior.state;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.helper;
using FarseerPhysics.Factories;
using wickedcrush.manager.audio;
using wickedcrush.display._3d;
using Com.Brashmonkey.Spriter.player;
using wickedcrush.particle;
using wickedcrush.map.path;
using wickedcrush.entity.physics_entity.agent.action;
using wickedcrush.inventory;

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public struct PositionReaction
    {
        public Vector2 pos;
        public Timer reactionTime;

        public PositionReaction(Vector2 pos, int ms)
        {
            this.pos = pos;
            reactionTime = new Timer(ms);
            reactionTime.resetAndStart();
        }
    }
    public class Murderer : Agent
    {
        private Color testColor = Color.Green;

        private int attackTellLength = 600, postAttackLength = 700, navigationResetLength = 500, attackRange = 30, returnLength = 2500, nodeWait = 2500, patrolLength = 7500, otherDirectionLength = 1300, hitReactionLength = 700;

        private float walkSpeed = 60f, runSpeed = 85f;

        public EnemyState enemyState = EnemyState.Idle;

        Stack<PathNode> patrol;

        List<PositionReaction> reactions = new List<PositionReaction>();

        Item item;

        public enum EnemyState
        {
            Idle,
            Patrol,
            Alert,
            Search,
            Return
        }

        public Murderer(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, PersistedStats stats, SoundManager sound, Stack<PathNode> patrol)
            : base(w, pos, size, center, solid, factory, stats, sound)
        {
            
            Initialize();
            this.stats = stats;
            this.patrol = patrol;
        }

        private void Initialize()
        {
            //stats = new PersistedStats(10, 10, 5);
            this.name = "Murderer";

            timers.Add("navigation", new Timer(navigationResetLength));
            timers["navigation"].start();

            timers.Add("attack_tell", new Timer(attackTellLength));
            timers.Add("post_attack", new Timer(postAttackLength));
            timers.Add("search_return", new Timer(returnLength));
            timers.Add("node_wait", new Timer(nodeWait));
            timers.Add("patrol", new Timer(patrolLength));
            timers.Add("turn_around", new Timer(otherDirectionLength));

            attackRange = (int)size.X + 12;
            //timers.Add("falling", new Timer(500));

            startingFriction = 0.5f;
            stoppingFriction = 0.01f; //1+ is friction city, 1 is a lotta friction, 0.1 is a little slippery, 0.01 is quite slip


            activeRange = 300f;
            this.targetSpeed = walkSpeed;

            SetupStateMachine();

            InitializeHpBar();


            item = ItemServer.getItem("Spellbook: Fireball");
            stats.inventory.receiveItem(item);

            subEntityList.Add("status", new TextEntity(enemyState.ToString(), pos, _sound, factory._game, -1, factory, 2f, 2f, 0f));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (remove)
            {
                return;
            }

            UpdateHpBar();
            UpdateAnimation();

            movementDirection = (int)facing;

            if (!this.staggered)
            {
                for (int i = reactions.Count - 1; i >= 0; i--)
                {
                    reactions[i].reactionTime.Update(gameTime);

                    if (reactions[i].reactionTime.isDone())
                    {
                        if (enemyState != EnemyState.Alert)
                        {
                            enemyState = EnemyState.Search;
                            //timers["turn_around"].reset();
                            searchPosition = reactions[i].pos;
                        }

                        reactions.Remove(reactions[i]);
                    }
                }
            }

            stats.set("boost", 1000);
            //stats.set("charge", 1000);

            subEntityList["status"].pos = this.pos;
            ((TextEntity)subEntityList["status"]).text = enemyState.ToString();
            
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            ctrl.Add("falling",
                new State("falling",
                    c => ((Murderer)c).timers["falling"].isActive(),
                    c =>
                    {
                        //((Murderer)c).RemoveOverheadWeapon("longsword");
                        this.height -= 3;
                    }));
            ctrl.Add("staggered",
                new State("staggered",
                    c => ((Murderer)c).staggered,
                    c =>
                    {
                        if (!stateTree.previousControlState.name.Equals("staggered"))
                        {
                            //((Murderer)c).RemoveOverheadWeapon("longsword");
                            this.PlayCue("vanquished");
                            item.Cancel((Agent)c);
                        }
                        
                        testColor = Color.White;
                        ResetAllTimers();

                        _sound.setGlobalVariable("InCombat", 1f);
                    }));
            ctrl.Add("post_attack",
                new State("post_attack",
                    c => ((Murderer)c).timers["post_attack"].isActive(),
                    c =>
                    {
                        /*if (!timers["navigation"].isActive())
                        {
                            timers["navigation"].resetAndStart();
                        }

                        if (timers["navigation"].isDone())
                        {
                            createPathToTarget();
                            timers["navigation"].resetAndStart();
                        }

                        this.speed = walkSpeed;
                        FollowPath(false);*/

                        if (!stateTree.previousControlState.name.Equals("post_attack"))
                        {
                            //((Murderer)c).RemoveOverheadWeapon("longsword");
                            //this.PlayCue("explosion");
                        }
                        testColor = Color.Violet;
                        if (timers["post_attack"].isDone())
                        {
                            timers["post_attack"].reset();
                        }

                        _sound.setGlobalVariable("InCombat", 1f);
                    }));
            ctrl.Add("attack_tell",
                new State("attack_tell",
                    c => ((Murderer)c).timers["attack_tell"].isActive(),
                    c =>
                    {
                        if (!stateTree.previousControlState.name.Equals("attack_tell"))
                        {
                            //((Murderer)c).AddOverheadWeapon("longsword", "weapons", "sword", 0, new Vector2(0f, 120f), .5f, 90f);
                            faceTarget();
                            ParticleStruct ps = new ParticleStruct(new Vector3(pos.X + center.X - 3f, height + 30, pos.Y + center.Y - 3f), new Vector3(6f, 0f, 6f), new Vector3(-3f, 5f, -3f), new Vector3(6f, 0f, 6f), new Vector3(0f, -0.01f, 0f), 0f, 0f, 500f, "particles", 0, "white_to_yellow");  
                            this.EmitParticles(ps, 5);
                            PlayCue("chime");
                            item.Press((Agent)c);
                        }

                        item.Hold((Agent)c);

                        /*if (!timers["navigation"].isActive())
                        {
                            timers["navigation"].resetAndStart();
                        }

                        if (timers["navigation"].isDone())
                        {
                            createPathToTarget();
                            timers["navigation"].resetAndStart();
                        }

                        this.speed = walkSpeed;
                        FollowPath(false);*/

                        testColor = Color.Yellow;
                        if(timers["attack_tell"].isDone())
                        {
                            //attackForward(size*2f, 30, 500);
                            //((Murderer)c).useActionSkill(SkillServer.skills["Swordz Attack"]);
                            item.Release((Agent)c);
                            testColor = Color.Red;
                            timers["post_attack"].resetAndStart();
                            timers["attack_tell"].reset();
                        }

                        _sound.setGlobalVariable("InCombat", 1f);
                    }));
            ctrl.Add("search",
                new State("search",
                    c => ((Murderer)c).enemyState == EnemyState.Search,
                    c =>
                    {
                        if (!timers["navigation"].isActive())
                        {
                            timers["navigation"].resetAndStart();
                        }

                        if (timers["navigation"].isDone())
                        {
                            createPathToLocation(searchPosition);
                            timers["navigation"].resetAndStart();
                        }
                        this.targetSpeed = walkSpeed;


                        FollowPath(false);

                        if (target == null)
                        {
                            setTargetToClosestPlayer(true, 120);
                            if (target != null)
                            {
                                factory.addText("!", pos + center + new Vector2(0, -10), 500);
                                TellProximityTarget(target);
                                enemyState = EnemyState.Alert;
                                _sound.playCue("0x53");
                            }
                        }

                        if (distanceToSearchPos() < 40)
                        {
                            if (!timers["search_return"].isActive())
                            {
                                timers["turn_around"].resetAndStart();
                                timers["search_return"].resetAndStart();
                                factory.addText("?", pos + center + new Vector2(0, -10), 500);
                            }

                            if (timers["search_return"].isDone())
                            {
                                enemyState = EnemyState.Return;
                                _sound.playCue("0x84");
                                timers["search_return"].reset();
                            }
                            if (timers["turn_around"].isDone())
                            {
                                facing = Helper.constrainDirection(facing + 135);
                                timers["turn_around"].resetAndStart();
                            }

                            
                        }

                    }
            ));
            ctrl.Add("return",
                new State("return",
                    c => ((Murderer)c).enemyState == EnemyState.Return,
                    c =>
                    {
                        if (!timers["navigation"].isActive())
                        {
                            timers["navigation"].resetAndStart();
                        }

                        if (timers["navigation"].isDone())
                        {
                            createPathToLocation(initialPosition);
                            timers["navigation"].resetAndStart();
                        }

                        this.targetSpeed = walkSpeed;
                        FollowPath(false);

                        if (target == null)
                        {
                            setTargetToClosestPlayer(true, 110);
                            if (target != null)
                            {
                                factory.addText("!", pos + center + new Vector2(0, -10), 500);
                                TellProximityTarget(target);
                                enemyState = EnemyState.Alert;
                                _sound.playCue("0x53");
                            }
                        }

                        if (distanceToPosition(initialPosition) < 40)
                        {
                            enemyState = EnemyState.Idle;
                            //_sound.playCue("0x84");
                        }

                        timers["search_return"].reset();

                    }
            ));
            ctrl.Add("chase",
                new State("chase",
                    c => ((Murderer)c).enemyState == EnemyState.Alert,
                    c =>
                    {
                        if (!timers["navigation"].isActive())
                        {
                            timers["navigation"].resetAndStart();
                        }

                        if (timers["navigation"].isDone())
                        {
                            TellProximityTarget(target);
                            createPathToTarget();
                            timers["navigation"].resetAndStart();
                        }

                        this.targetSpeed = runSpeed;

                        if (distanceToTarget() > attackRange)
                        {

                            FollowPath(false);


                            testColor = Color.Green;

                            _sound.setGlobalVariable("InCombat", 1f);

                            
                        }
                        else if (distanceToTarget() >= 0)
                        {
                            timers["attack_tell"].resetAndStart();
                        }

                        if (target == null)
                        {
                            enemyState = EnemyState.Idle;
                        }
                        else if (distanceToTarget() > activeRange || !hasLineOfSightToEntity(target))
                        {
                            searchPosition = target.pos + target.center;
                            target = null;
                            enemyState = EnemyState.Search;

                            _sound.playCue("0x88");
                        }

                        timers["search_return"].reset();
                    }));
            ctrl.Add("patrol",
                new State("patrol",
                    c => ((Murderer)c).enemyState == EnemyState.Patrol,
                    c =>
                    {
                        if (path == null || path.Count == 0)
                        {
                            path = new Stack<PathNode>(patrol);
                        }

                        this.targetSpeed = walkSpeed;

                        FollowPath(false);

                        if (target == null)
                        {
                            setTargetToClosestPlayer(true, 90);
                            if (target != null)
                            {
                                factory.addText("!", pos + center + new Vector2(0, -10), 500);
                                TellProximityTarget(target);
                                enemyState = EnemyState.Alert;
                                _sound.playCue("0x53");
                            }
                            else
                            {
                                if (!timers["patrol"].isActive())
                                {
                                    timers["patrol"].resetAndStart();
                                }
                                if (timers["patrol"].isDone())
                                {
                                    timers["patrol"].reset();
                                    enemyState = EnemyState.Idle;
                                }
                            }
                        }

                        timers["search_return"].reset();
                        
                    }));

            ctrl.Add("idle",
                new State("idle",
                    c => true,
                    c =>
                    {
                        
                            setTargetToClosestPlayer(true, 90);
                            if (target != null)
                            {
                                factory.addText("!", pos + center + new Vector2(0, -10), 500);
                                TellProximityTarget(target);
                                enemyState = EnemyState.Alert;
                                _sound.playCue("0x53");
                                _sound.setGlobalVariable("InCombat", 1f);

                                if (distanceToTarget() < attackRange)
                                {
                                    timers["attack_tell"].resetAndStart();
                                    
                                }
                                
                            }
                            else
                            {
                                if (patrol.Count > 0)
                                {

                                    if (!timers["node_wait"].isActive())
                                    {
                                        timers["turn_around"].resetAndStart();
                                        timers["node_wait"].resetAndStart();
                                    }
                                    if (timers["node_wait"].isDone())
                                    {
                                        timers["node_wait"].reset();
                                        enemyState = EnemyState.Patrol;
                                    }
                                    if (timers["turn_around"].isDone())
                                    {
                                        facing = Helper.constrainDirection(facing + 135);
                                        timers["turn_around"].resetAndStart();
                                    }

                                }
                            }

                            timers["search_return"].reset();

                        
                    }));
            stateTree = new StateTree();
            stateTree.AddBranch("default", new StateBranch(c => true, ctrl));
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("you", new SpriterPlayer(factory._spriterManager.spriters["you"].getSpriterData(), 0, factory._spriterManager.spriters["you"].loader));
            bodySpriter = sPlayers["you"];
            bodySpriter.setFrameSpeed(20);

        }

        protected void TellProximityTarget(Entity target)
        {
            if (target == null)
                return;

            foreach (Entity e in proximity)
            {
                if (e is Murderer)
                {
                    ((Murderer)e).reactions.Add(new PositionReaction(target.pos + target.center, 1000));
                }
            }
        }

        protected void UpdateAnimation()
        {
            
            //if (sPlayer == null)
            //return;

            String bad = "";

            if (timers["falling"].isActive())
            {
                bodySpriter.setAnimation("fall", 0, 0);
                return;
            }

            switch (facing)
            {
                case Direction.East:
                    bad += "east";

                    //bodySpriter.setAnimation("east", 0, 0);
                    break;

                case Direction.North:
                    bad += "north";
                    //bodySpriter.setAnimation("north", 0, 0);
                    break;

                case Direction.South:
                    bad += "south";
                    //bodySpriter.setAnimation("south", 0, 0);
                    break;

                case Direction.West:
                    bad += "west";
                    //bodySpriter.setAnimation("west", 0, 0);
                    break;

                case Direction.NorthEast:
                    bad += "northeast";
                    //bodySpriter.setAnimation("northeast", 0, 0);
                    break;

                case Direction.NorthWest:
                    bad += "northwest";
                    //bodySpriter.setAnimation("northwest", 0, 0);
                    break;

                case Direction.SouthEast:
                    bad += "southeast";
                    //bodySpriter.setAnimation("southeast", 0, 0);
                    break;

                case Direction.SouthWest:
                    bad += "southwest";
                    //bodySpriter.setAnimation("southwest", 0, 0);
                    break;
            }

            if (bodies["body"].LinearVelocity.Length() >= 1f)
            {
                bad += "_run";
            }
            else
            {
                bad += "_stand";
            }

            bodySpriter.setAnimation(bad, 0, 0);
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {
            //if (navigator != null)
            //{
                //navigator.DebugDraw(wTex, gd, spriteBatch, f);
            //}

            spriteBatch.Draw(wTex, bodies["body"].Position - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, testColor, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(aTex, pos + center - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, testColor, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, hotSpot.WorldCenter, null, Color.Yellow, hotSpot.Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            
            DrawName(spriteBatch, f, camera);

            DebugDrawHealth(wTex, aTex, gd, spriteBatch, f, c, camera);
        }

        public override void TakeSkill(ActionSkill action)
        {
            if (this.immortal || remove || hitThisTick)
                return;

            hitThisTick = true;

            foreach (KeyValuePair<string, int> pair in action.statIncrement)
            {
                int amount = pair.Value;

                if (pair.Key.Equals("hp"))
                {
                    if (this.staggered)
                    {
                        amount *= 2;
                    }
                    if (enemyState == EnemyState.Idle || enemyState == EnemyState.Patrol)
                    {
                        
                        
                        amount *= 4;

                        if (action.parent != null)
                        {
                            reactions.Add(new PositionReaction(action.parent.pos + action.parent.center, hitReactionLength));
                        }

                    }
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
            particleEmitter.EmitParticles(ps, this.factory, 3);

            //Vector2 bloodspurt = new Vector2((float)(Math.Cos(MathHelper.ToRadians((float)action.facing)) + 0f * Math.Sin(MathHelper.ToRadians((float)action.facing))),
                //(float)(Math.Sin(MathHelper.ToRadians((float)action.facing)) - 0f * Math.Cos(MathHelper.ToRadians((float)action.facing))));

            //ps = new ParticleStruct(new Vector3(action.pos.X + action.center.X, action.height + 10, action.pos.Y + action.center.Y), Vector3.Zero, new Vector3(bloodspurt.X * 3f, 2f, bloodspurt.Y * 3f), new Vector3(1.5f, 4f, -1.5f), new Vector3(0, -.3f, 0), 0f, 0f, 500, "particles", 3, "bloodspurt_002");
            //particleEmitter.EmitParticles(ps, this.factory, 5);

            Vector2 v = bodies["body"].LinearVelocity;

            Vector2 unitVector = new Vector2(
                (float)Math.Cos(MathHelper.ToRadians((float)action.force.Key)),
                (float)Math.Sin(MathHelper.ToRadians((float)action.force.Key)));

            float staggerMultiplier = 30f;

            if (!staggered)
            {
                if (enemyState == EnemyState.Idle || enemyState == EnemyState.Patrol)
                {
                    staggerMultiplier *= 8f;
                    stats.addTo("stagger", action.force.Value * 2);
                }
                else
                {
                    stats.addTo("stagger", action.force.Value);
                }

                
            } else {
                staggerMultiplier *= 2f;
            }

            


            v.X += unitVector.X * (float)action.force.Value * staggerMultiplier * (float)stats.get("staggerDistance");
            v.Y += unitVector.Y * (float)action.force.Value * staggerMultiplier * (float)stats.get("staggerDistance");

            if (bodies.ContainsKey("body"))
                bodies["body"].LinearVelocity = v;

            action.PlayTakeSound();

            factory._gm.camera.ShakeScreen(5f);
        }
    }
}
