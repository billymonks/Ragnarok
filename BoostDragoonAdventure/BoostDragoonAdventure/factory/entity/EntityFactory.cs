using System;
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

namespace wickedcrush.factory.entity
{
    public class EntityFactory
    {
        private GameBase _game; //lol what r u gonna do about it ;) jk pls be nice
        private EntityManager _em;
        private PlayerManager _pm;
        public GameplayManager _gm;
        private ControlsManager _cm;
        private SoundManager _sm;
        public World _w;
        public RoomManager _rm;
        public SpriterManager _spriterManager;

        private List<Door> doorList;

        public EntityFactory(GameBase game, GameplayManager gm, EntityManager em, RoomManager rm, World w)
        {

            this._game = game;
            this._em = em;
            this._pm = _game.playerManager;
            this._gm = gm;
            this._cm = _game.controlsManager;
            this._sm = _game.soundManager;
            this._rm = rm;
            this._w = w;
            this._spriterManager = game.spriterManager;

            doorList = new List<Door>();
        }

        public void createEntity(Vector2 pos, Vector2 size, Vector2 center)
        {
            Entity e = new Entity(pos, size, center, _sm);
            
            _em.addEntity(e);
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

        public void addMurderer(Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            PersistedStats fuckStats = new PersistedStats();
            Murderer a = new Murderer(_w, pos, size, center, solid, this, fuckStats, _sm);
            a.stats.set("hp", 40);
            a.stats.set("maxHP", 40);
            a.stats.set("staggerLimit", 50);
            a.stats.set("stagger", 0);
            a.stats.set("staggerDuration", 30);
            a.stats.set("staggerDistance", 1);
            if (_gm.map != null)
            {
                a.activateNavigator(_gm.map);
            }
            _em.addEntity(a);
        }

        public void addTurret(Vector2 pos, Direction facing)
        {
            Turret t = new Turret(_w, pos, this, facing, _sm);
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
            FloorSwitch f = new FloorSwitch(_w, pos, this, _sm);

            f.stats.set("staggerDuration", 1);
            f.stats.set("staggerDistance", 0);
            
            _em.addEntity(f);
        }

        public void addTimerTrigger(Vector2 pos)
        {
            TimerTrigger t = new TimerTrigger(_w, pos, this, _sm);

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
            ActionSkill a = new ActionSkill(skillStruct, _game, _gm, parent);

            _em.addEntity(a);
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
