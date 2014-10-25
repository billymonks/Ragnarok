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
using wickedcrush.manager.map.room;
using wickedcrush.entity.physics_entity.agent.trap;

namespace wickedcrush.factory.entity
{
    public class EntityFactory
    {
        private EntityManager em;
        private PlayerManager pm;
        private MapManager mm;
        private ControlsManager cm;
        private SoundManager sm;
        private World w;
        public RoomManager rm;

        private List<Door> doorList;

        //private Map map;

        public EntityFactory(EntityManager em, PlayerManager pm, MapManager mm, ControlsManager cm, SoundManager sm, RoomManager rm, World w)
        {
            this.em = em;
            this.pm = pm;
            this.mm = mm;
            this.cm = cm;
            this.sm = sm;
            this.rm = rm;
            this.w = w;

            doorList = new List<Door>();
        }

        /*public void setMap(Map map)
        {
            this.map = map;
        }

        public void clearMap()
        {
            map = null;
        }*/

        public void createEntity(Vector2 pos, Vector2 size, Vector2 center)
        {
            Entity e = new Entity(pos, size, center, sm);

            //e.stats.set("staggerDuration", 0);
            //e.stats.set("staggerDistance", 0);
            
            em.addEntity(e);
        }

        public void addDoor(Vector2 pos, Direction facing, Connection connection)
        {
            Door d = new Door(w, pos, facing, connection, mm, this, sm);
            doorList.Add(d);

            em.addEntity(d);
        }

        public void addPhysicsEntity(Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            PhysicsEntity e = new PhysicsEntity(w, pos, size, center, solid, sm);

            //e.stats.set("staggerDuration", 0);
            //e.stats.set("staggerDistance", 0);
            
            em.addEntity(e);
        }

        public PlayerAgent addPlayerAgent(String name, Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls c, PersistedStats stats)
        {
            PlayerAgent agent = new PlayerAgent(w, pos, size, center, solid, c, stats, this, sm, name);

            /*agent.stats.set("maxBoost", 1000);
            agent.stats.set("boost", 1000);
            agent.stats.set("fillSpeed", 3);
            agent.stats.set("useSpeed", 8);
            agent.stats.set("boostSpeedMod", 0);

            agent.stats.set("boostRecharge", 250);
            agent.stats.set(("iFrameTime"), 150);
            agent.stats.set("staggerDistance", 100);*/
            
            em.addEntity(agent);
            
            return agent;
        }

        public void addMurderer(Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            PersistedStats fuckStats = new PersistedStats();
            Murderer a = new Murderer(w, pos, size, center, solid, this, fuckStats, sm);
            a.stats.set("hp", 80);
            a.stats.set("maxHP", 80);
            a.stats.set("staggerLimit", 100);
            a.stats.set("stagger", 0);
            a.stats.set("staggerDuration", 30);
            a.stats.set("staggerDistance", 1);
            if (mm.map != null)
            {
                a.activateNavigator(mm.map);
            }
            em.addEntity(a);
        }

        public void addTurret(Vector2 pos, Direction facing)
        {
            Turret t = new Turret(w, pos, this, facing, sm);
            t.stats.set("hp", 20);
            t.stats.set("maxHP", 20);
            t.stats.set("staggerDuration", 1);
            t.stats.set("staggerDistance", 0);
            em.addEntity(t);
        }

        public void addAimTurret(Vector2 pos)
        {
            AimTurret t = new AimTurret(w, pos, this, Direction.East, sm);
            t.stats.set("hp", 20);
            t.stats.set("maxHP", 20);
            t.stats.set("staggerDuration", 1);
            t.stats.set("staggerDistance", 0);
            em.addEntity(t);
        }

        public void addChest(Vector2 pos)
        {
            Chest c = new Chest(w, pos, this, sm);
            c.stats.set("staggerDuration", 1);
            c.stats.set("staggerDistance", 0);
            em.addEntity(c);
        }

        public void addBolt(Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force)
        {
            Bolt b = new Bolt(w, pos, size, center, parent, damage, force, sm);

            b.stats.set("staggerDuration", 1);
            b.stats.set("staggerDistance", 0);

            em.addEntity(b);
        }

        public void addAimedProjectile(Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force, int aimDirection)
        {
            AimedProjectile b = new AimedProjectile(w, pos, size, center, parent, damage, force, aimDirection, sm);

            b.stats.set("staggerDuration", 1);
            b.stats.set("staggerDistance", 0);

            em.addEntity(b);
        }

        public void addFireball(Vector2 pos, Vector2 size, Vector2 center, Entity parent, Direction facing, int damage, int force, int clusters)
        {
            Fireball b = new Fireball(w, pos, size, center, parent, facing, damage, force, clusters, sm, this);

            b.stats.set("staggerDuration", 1);
            b.stats.set("staggerDistance", 0);

            em.addEntity(b);
        }

        public void addFloorSwitch(Vector2 pos)
        {
            FloorSwitch f = new FloorSwitch(w, pos, this, sm);

            f.stats.set("staggerDuration", 1);
            f.stats.set("staggerDistance", 0);
            
            em.addEntity(f);
        }

        public void addTimerTrigger(Vector2 pos)
        {
            TimerTrigger t = new TimerTrigger(w, pos, this, sm);

            t.stats.set("staggerDuration", 1);
            t.stats.set("staggerDistance", 0);
            
            em.addEntity(t);
        }

        public void addMeleeAttack(Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force)
        {
            MeleeAttack a = new MeleeAttack(w, pos, size, center, parent, damage, force, sm);

            a.stats.set("staggerDuration", 1);
            a.stats.set("staggerDistance", 0);
            
            em.addEntity(a);
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
            
            
            foreach (Player p in pm.getPlayerList())
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


            foreach (Player p in pm.getPlayerList())
            {
                if ((p.getAgent() == null || p.getAgent().readyForRemoval()) && p.c.StartPressed())
                {
                    p.getStats().set("hp", p.getStats().get("maxHP"));
                    p.GenerateAgent(doorList[0].pos + current.Value, new Vector2(24, 24), new Vector2(12, 12), true, this);
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
            w.Step(0f);
            //w.ProcessChanges();
        }
    }
}
