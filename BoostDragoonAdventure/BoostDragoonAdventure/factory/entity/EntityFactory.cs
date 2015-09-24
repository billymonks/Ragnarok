﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.entity;
using FarseerPhysics.Dynamics;
using wickedcrush.entity;
using Microsoft.Xna.Framework;
using wickedcrush.entity.physics_entity;
using wickedcrush.entity.physics_entity.agent;
using wickedcrush.stats;
using wickedcrush.entity.physics_entity.agent.attack;
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.controls;
using wickedcrush.map;
using wickedcrush.manager.player;
using wickedcrush.manager.controls;
using wickedcrush.player;
using wickedcrush.entity.physics_entity.agent.attack.projectile;
using wickedcrush.entity.physics_entity.agent.attack.melee;
using wickedcrush.entity.physics_entity.agent.enemy;
using wickedcrush.entity.physics_entity.agent.trap.triggerable.turret;
using wickedcrush.entity.physics_entity.agent.chest;
using wickedcrush.entity.physics_entity.agent.trap.trigger;
using wickedcrush.manager.audio;
using wickedcrush.entity.physics_entity.agent.inanimate;
using wickedcrush.manager.gameplay;
using wickedcrush.entity.physics_entity.agent.trap;
using wickedcrush.entity.physics_entity.agent.npc;
using wickedcrush.manager.gameplay.room;
using wickedcrush.manager.map;
using wickedcrush.display.spriter;
using wickedcrush.entity.physics_entity.agent.action;
using wickedcrush.utility;
using wickedcrush.manager.particle;
using wickedcrush.screen;
using wickedcrush.screen.dialog;
using wickedcrush.entity.holder;
using wickedcrush.map.path;
using wickedcrush.entity.physics_entity.agent.trap.triggerable;
using Com.Brashmonkey.Spriter.player;

namespace wickedcrush.factory.entity
{
    public class EntityFactory
    {
        public GameBase _game; //lol what r u gonna do about it ;) jk pls be nice
        private EntityManager _em;
        private PlayerManager _pm;
        public GameplayManager _gm;
        private ControlsManager _cm;
        public ParticleManager _particleManager;
        public SoundManager _sm;
        public World _w;
        public RoomManager _rm;
        public SpriterManager _spriterManager;

        public Random random;

        private List<Door> doorList;

        public Dictionary<String, bool> savedBools = new Dictionary<String, bool>();

        public EntityFactory(GameBase game, GameplayManager gm, EntityManager em, ParticleManager particleManager, RoomManager rm, World w)
        {

            this._game = game;
            this._em = em;
            this._pm = _game.playerManager;
            this._gm = gm;
            this._particleManager = particleManager;
            this._cm = _game.controlsManager;
            this._sm = _game.soundManager;
            this._rm = rm;
            this._w = w;
            this._spriterManager = game.spriterManager;

            random = new Random();

            doorList = new List<Door>();
        }

        public bool checkBool(string key)
        {
            if (savedBools.ContainsKey(key))
                return savedBools[key];

            return false;
        }

        public void createTextScreen(String text, Vector2 pos)
        {
            _game.screenManager.AddScreen(new TextDisplayScreen(_game, _pm.getPlayerList()[0], text, pos));
        }

        //needs some kind of screen manager / dialog manager
        public void createDialog(String dialog, Vector2 pos)
        {
            Player p = _pm.getPlayerList()[0];
            GameScreen root = new TextDisplayScreen(_game, p, "", pos);
            GameScreen pointer = root;
            

            dialog = dialog.Replace("\r\n", "¥");

            dialog = dialog.Replace("\\n", "\n");

            string[] text = dialog.Split('¥');

            string[] temps;
            string temp1, temp2;
            //int tempInt;

            //bool inBranch = false;
            Stack<StatBranchScreen> branch = new Stack<StatBranchScreen>();
            

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].StartsWith("say"))
                {
                    setNextScreen(branch, ref pointer, new TextDisplayScreen(_game, p, text[i].Substring(4, text[i].Length - 5), pos));
                    //pointer = pointer.nextScreen;
                    //((TextDisplayScreen)pointer).text = text[i].Substring(4, text[i].Length - 5);
                }
                else if (text[i].StartsWith("setvar"))
                {
                    temps = text[i].Split('|');
                    temp1 = temps[0].Substring(temps[0].IndexOf('[')+1, temps[0].Length - temps[0].IndexOf('[')-1);
                    temp2 = temps[1].Substring(0, temps[1].IndexOf(']'));
                    setNextScreen(branch, ref pointer, new StatSetScreen(_game, p, temp1, int.Parse(temp2)));
                    //pointer = pointer.nextScreen;
                }
                else if (text[i].StartsWith("branch"))
                {
                    temps = text[i].Split('|');
                    temp1 = temps[0].Substring(temps[0].IndexOf('[') + 1, temps[0].Length - temps[0].IndexOf('[') - 1);
                    temp2 = temps[1].Substring(0, temps[1].IndexOf(']'));
                    branch.Push(new StatBranchScreen(_game, p, temp1, int.Parse(temp2)));
                    pointer.nextScreen = branch.Peek();
                    //pointer = 
                }
                else if (text[i].StartsWith("yes"))
                {
                    branch.Peek().branchInt = 0;
                }
                else if (text[i].StartsWith("no"))
                {
                    branch.Peek().branchInt = 1;
                }
                else if (text[i].StartsWith("end"))
                {
                    branch.Peek().branchInt = -1;
                }
            }
            _game.screenManager.AddScreen(root.nextScreen);
        }

        private void setNextScreen(Stack<StatBranchScreen> branchStack, ref GameScreen pointer, GameScreen nextScreen)
        {
            StatBranchScreen temp;
            if (branchStack.Count > 0 && branchStack.Peek().branchInt == -1)
            {
                temp = branchStack.Pop();
                temp.AddToNo(nextScreen);
                temp.AddToYes(nextScreen);
                pointer = nextScreen;
            }
            else if (branchStack.Count > 0 && branchStack.Peek().branchInt == 0)
            {
                temp = branchStack.Peek();
                temp.AddToYes(nextScreen);
            }
            else if (branchStack.Count > 0 && branchStack.Peek().branchInt == 1)
            {
                temp = branchStack.Peek();
                temp.AddToNo(nextScreen);
            }
            else
            {
                pointer.nextScreen = nextScreen;
                pointer = pointer.nextScreen;
            }
        }

        public void createBooleanChoiceScreen(String text, Vector2 pos, string key)
        {
            //if(!savedBools.ContainsKey(key))
                //savedBools.Add(key, false);
            //_game.screenManager.AddScreen(new BooleanChoiceScreen(_game, _pm.getPlayerList()[0], text, pos, key, this));
        }

        public void createEntity(Vector2 pos, Vector2 size, Vector2 center)
        {
            Entity e = new Entity(pos, size, center, _sm);
            
            _em.addEntity(e);
        }

        public void AddSpriterToHud(SpriterPlayer s)
        {
            _gm._screen.AddSpriter(s);
        }

        public void RemoveSpriterFromHud(SpriterPlayer s)
        {
            _gm._screen.RemoveSpriter(s);
        }

        public CursorEntity addCursor()
        {
            CursorEntity c = new CursorEntity(_w, _sm, _game, this);

            _em.addEntity(c);

            return c;
        }

        public void addDoor(Vector2 pos, Direction facing, Connection connection)
        {
            Door d = new Door(_w, pos, facing, connection, _gm, this, _sm);
            doorList.Add(d);

            _em.addEntity(d);
        }

        public void addDestination(Vector2 pos)
        {
            DestinationAgent d = new DestinationAgent(pos, new Vector2(80f, 80f), _gm, _game);

            _em.addEntity(d);
        }

        public void addPhysicsEntity(Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            PhysicsEntity e = new PhysicsEntity(_w, pos, size, center, solid, _sm);
            
            _em.addEntity(e);
        }

        public PlayerAgent addPlayerAgent(String name, Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls c, PersistedStats stats, Player player)
        {
            PlayerAgent agent = new PlayerAgent(_w, pos, size, center, solid, c, stats, this, _sm, name, player);
            
            _em.addEntity(agent);
            
            return agent;
        }

        public void addGiant(Vector2 pos)
        {
            Giant g = new Giant(_w, pos, new Vector2(48, 48), new Vector2(24, 24), true, this, new PersistedStats(), _sm);
            g.stats.set("hp", 2000);
            g.stats.set("maxHP", 2000);
            g.stats.set("staggerLimit", 4500);
            g.stats.set("stagger", 0);
            g.stats.set("staggerDuration", 240);
            g.stats.set("staggerDistance", 0);
            if (_gm.map != null)
            {
                g.activateNavigator(_gm.map);
            }
            _em.addEntity(g);
        }

        public void addPathOfDeath(Vector2 pos, Vector2 size, Stack<PathNode> patrol)
        {
            PathOfDeath p = new PathOfDeath(_w, pos, size, size / 2f, true, this, _sm, patrol, 15, 100);
            _em.addEntity(p);
        }

        public void addMurderer(Vector2 pos, Vector2 size, bool solid, Stack<PathNode> patrol, int facing)
        {
            PersistedStats fuckStats = new PersistedStats();
            Murderer a = new Murderer(_w, pos, size, size/2f, solid, this, fuckStats, _sm, patrol);
            a.stats.set("hp", 280);
            a.stats.set("maxHP", 280);
            a.stats.set("staggerLimit", 500);
            a.stats.set("stagger", 0);
            a.stats.set("staggerDuration", 60);
            a.stats.set("staggerDistance", 1);
            a.facing = helper.Helper.constrainDirection((Direction)facing);
            if (_gm.map != null)
            {
                a.activateNavigator(_gm.map);
            }
            _em.addEntity(a);
        }

        public void addWeakling(Vector2 pos, Vector2 size, Vector2 center, Stack<PathNode> patrol)
        {
            PersistedStats fuckStats = new PersistedStats();
            Murderer a = new Murderer(_w, pos, size, center, true, this, fuckStats, _sm, patrol);
            a.stats.set("hp", 100);
            a.stats.set("maxHP", 100);
            a.stats.set("staggerLimit", 300);
            a.stats.set("stagger", 0);
            a.stats.set("staggerDuration", 60);
            a.stats.set("staggerDistance", 1);
            if (_gm.map != null)
            {
                a.activateNavigator(_gm.map);
            }
            _em.addEntity(a);
        }

        public void addShiftyShooter(Vector2 pos, Vector2 size, Vector2 center, int spreadDuration, int blowCount, int blowPerSpread, int scatterCount, int spread, float blowVelocity, int blowDuration, int blowReleaseDelay, int moveLength, int standLength, int standToShootLength, float skillVelocity)
        {
            PersistedStats fuckStats = new PersistedStats();
            ShiftyShooter s = new ShiftyShooter(_w, pos, size, center, true, this, fuckStats, _sm, spreadDuration, blowCount, blowPerSpread, scatterCount, spread, blowVelocity, blowDuration, blowReleaseDelay, moveLength, standLength, standToShootLength, skillVelocity);
            s.stats.set("hp", 100);
            s.stats.set("maxHP", 100);
            s.stats.set("staggerLimit", 500);
            s.stats.set("stagger", 0);
            s.stats.set("staggerDuration", 60);
            s.stats.set("staggerDistance", 1);

            _em.addEntity(s);
        }

        public void addTurret(Vector2 pos, Direction facing)
        {
            StandaloneTurret t = new StandaloneTurret(_w, pos, this, facing, _sm);
            t.stats.set("hp", 20);
            t.stats.set("maxHP", 20);
            t.stats.set("staggerDuration", 1);
            t.stats.set("staggerDistance", 0);
            _em.addEntity(t);
        }

        public void addAimTurret(Vector2 pos)
        {
            AimTurret t = new AimTurret(_w, pos, this, Direction.East, _sm);
            t.stats.set("hp", 20);
            t.stats.set("maxHP", 20);
            t.stats.set("staggerDuration", 1);
            t.stats.set("staggerDistance", 0);
            _em.addEntity(t);
        }

        public void addChest(Vector2 pos)
        {
            Chest c = new Chest(_w, pos, this, _sm);
            c.stats.set("staggerDuration", 1);
            c.stats.set("staggerDistance", 0);
            _em.addEntity(c);
        }

        public void addNPC(Vector2 pos, Vector2 size, String dialog)
        {
            NPC npc = new NPC(pos, size, dialog, _game, _gm);
            npc.stats.set("staggerDuration", 1);
            npc.stats.set("staggerDistance", 0);
            _em.addEntity(npc);
        }

        public void addTerminal(Vector2 pos)
        {
            TerminalNPC c = new TerminalNPC(pos, _game, _gm);
            c.stats.set("staggerDuration", 1);
            c.stats.set("staggerDistance", 0);
            _em.addEntity(c);
        }

        public void addBolt(Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force)
        {
            Bolt b = new Bolt(_w, pos, size, center, parent, damage, force, _sm, this);

            b.stats.set("staggerDuration", 1);
            b.stats.set("staggerDistance", 0);

            _em.addEntity(b);
        }

        public void addAimedProjectile(Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force, int aimDirection)
        {
            AimedProjectile b = new AimedProjectile(_w, pos, size, center, parent, damage, force, aimDirection, _sm, this);

            b.stats.set("staggerDuration", 1);
            b.stats.set("staggerDistance", 0);

            _em.addEntity(b);
        }

        public void addFireball(Vector2 pos, Vector2 size, Vector2 center, Entity parent, Direction facing, int damage, int force, int clusters)
        {
            Fireball b = new Fireball(_w, pos, size, center, parent, facing, damage, force, clusters, _sm, this);

            b.stats.set("staggerDuration", 1);
            b.stats.set("staggerDistance", 0);

            _em.addEntity(b);
        }

        public void addFloorSwitch(Vector2 pos)
        {
            FloorTrap f = new FloorTrap(_w, pos, this, _sm);

            f.stats.set("staggerDuration", 1);
            f.stats.set("staggerDistance", 0);
            
            _em.addEntity(f);
        }

        public void addTimerTrigger(Vector2 pos, int time)
        {
            TimerTrigger t = new TimerTrigger(_w, pos, this, _sm, time);
            t.stats.set("hp", 20);
            t.stats.set("maxHP", 20);
            t.stats.set("staggerDuration", 1);
            t.stats.set("staggerDistance", 0);
            
            _em.addEntity(t);
        }

        public void addMeleeAttack(Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force)
        {
            MeleeAttack a = new MeleeAttack(_w, pos, size, center, parent, damage, force, _sm, this);

            a.stats.set("staggerDuration", 1);
            a.stats.set("staggerDistance", 0);
            
            _em.addEntity(a);
        }

        public void addActionSkill(SkillStruct skillStruct, Entity parent)
        {
            ActionSkill a = new ActionSkill(skillStruct, _game, _gm, parent, null);

            _em.addEntity(a);
        }

        public void addActionSkill(SkillStruct skillStruct, Entity parent, Entity actingParent, int aimDirection)
        {
            ActionSkill a = new ActionSkill(skillStruct, _game, _gm, parent, actingParent, aimDirection);

            _em.addEntity(a);
        }

        public void addActionSkill(SkillStruct skillStruct, Entity parent, Entity actingParent)
        {
            ActionSkill a = new ActionSkill(skillStruct, _game, _gm, parent, actingParent);

            _em.addEntity(a);
        }

        public void addBlowReleaser(Entity parent, Entity actingParent, List<KeyValuePair<Timer, SkillStruct>> orphanedBlows)
        {
            BlowReleaser releaser = new BlowReleaser(parent, actingParent, orphanedBlows, _sm, _gm);

            _em.addEntity(releaser);
        }

        public TextEntity addText(String text, Vector2 pos, int duration, Color textColor, float zoomLevel, String font)
        {
            TextEntity textEnt = new TextEntity(text, pos, _sm, _game, duration, this, textColor, zoomLevel, font);
            _em.addEntity(textEnt);

            return textEnt;
        }

        public TextEntity addText(String text, Vector2 pos, int duration, Color textColor, float zoomLevel)
        {
            TextEntity textEnt = new TextEntity(text, pos, _sm, _game, duration, this, textColor, zoomLevel);
            _em.addEntity(textEnt);

            return textEnt;
        }

        public TextEntity addText(String text, Vector2 pos, int duration)
        {
            TextEntity textEnt = new TextEntity(text, pos, _sm, _game, duration, this, 1f);
            _em.addEntity(textEnt);

            return textEnt;
        }

        public void spawnPlayers(int doorIndex)
        {
            LinkedList<Vector2> positions = new LinkedList<Vector2>();
            positions.AddLast(new Vector2(-24, -24));
            positions.AddLast(new Vector2(-24, 24));
            positions.AddLast(new Vector2(24, 24));
            positions.AddLast(new Vector2(24, -24));
            positions.AddLast(new Vector2(48, 0));
            LinkedListNode<Vector2> current = positions.First;
            
            
            foreach (Player p in _pm.getPlayerList())
            {
                if(doorList.Count > doorIndex)
                    p.GenerateAgent(doorList[doorIndex].pos + current.Value, new Vector2(24, 24), new Vector2(12, 12), true, this);
                else
                    p.GenerateAgent(new Vector2(-48, 400) + current.Value, new Vector2(24, 24), new Vector2(12, 12), true, this);
                current = current.Next;
            }
        }

        private void respawnPlayer() //needs a new home
        {
            LinkedList<Vector2> positions = new LinkedList<Vector2>();
            positions.AddLast(new Vector2(-24, -24));
            positions.AddLast(new Vector2(-24, 24));
            positions.AddLast(new Vector2(24, 24));
            positions.AddLast(new Vector2(24, -24));
            positions.AddLast(new Vector2(48, 0));
            LinkedListNode<Vector2> current = positions.First;


            foreach (Player p in _pm.getPlayerList())
            {
                if ((p.getAgent() == null || p.getAgent().readyForRemoval()) && p.c.StartPressed())
                {
                    p.getStats().set("hp", p.getStats().get("maxHP"));
                    p.GenerateAgent(p.respawnPoint.pos + p.respawnPoint.center, new Vector2(24, 24), new Vector2(12, 12), true, this);
                    current = current.Next;
                }
            }
        }

        public void Update() //debug,something else should have this loop
        {
            respawnPlayer();
        }

        public void processWorldChanges()
        {
            _w.Step(0f);
            //w.ProcessChanges();
        }
    }
}
