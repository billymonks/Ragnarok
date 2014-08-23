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

namespace wickedcrush.factory.entity
{
    public class EntityFactory
    {
        private EntityManager em;
        private PlayerManager pm;
        private ControlsManager cm;
        private World w;

        private Map map;

        public EntityFactory(EntityManager em, PlayerManager pm, ControlsManager cm, World w)
        {
            this.em = em;
            this.pm = pm;
            this.cm = cm;
            this.w = w;
        }

        public void setMap(Map map)
        {
            this.map = map;
        }

        public void clearMap()
        {
            map = null;
        }

        public void createEntity(Vector2 pos, Vector2 size, Vector2 center)
        {
            Entity e = new Entity(pos, size, center);
            em.addEntity(e);
        }

        public void addPhysicsEntity(Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            PhysicsEntity e = new PhysicsEntity(w, pos, size, center, solid);
            em.addEntity(e);
        }

        public PlayerAgent addPlayerAgent(Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls c, PersistedStats stats)
        {
            PlayerAgent agent = new PlayerAgent(w, pos, size, center, solid, c, stats, this);
            em.addEntity(agent);
            
            return agent;
        }

        public void addAgent(Vector2 pos, Vector2 size, Vector2 center, bool solid, PersistedStats stats)
        {
            Murderer a = new Murderer(w, pos, size, center, solid, this, stats);
            a.stats.set("staggerLimit", 100);
            a.stats.set("stagger", 0);
            if (map != null)
            {
                a.activateNavigator(map);
            }
            em.addEntity(a);
        }

        public void addTurret(Vector2 pos, Direction facing)
        {
            Turret t = new Turret(w, pos, this, facing);
            em.addEntity(t);
        }

        public void addChest(Vector2 pos)
        {
            Chest c = new Chest(w, pos, this);
            em.addEntity(c);
        }

        public void addBolt(Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force)
        {
            Bolt b = new Bolt(w, pos, size, center, parent, damage, force);
            em.addEntity(b);
        }

        public void addFloorSwitch(Vector2 pos)
        {
            FloorSwitch f = new FloorSwitch(w, pos, this);
            em.addEntity(f);
        }

        public void addTimerTrigger(Vector2 pos)
        {
            TimerTrigger t = new TimerTrigger(w, pos, this);
            em.addEntity(t);
        }

        public void addMeleeAttack(Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force)
        {
            MeleeAttack a = new MeleeAttack(w, pos, size, center, parent, damage, force);
            em.addEntity(a);
        }

        public void spawnPlayers()
        {
            LinkedList<Vector2> positions = new LinkedList<Vector2>();
            positions.AddLast(new Vector2(3, 315));
            positions.AddLast(new Vector2(3, 340));
            positions.AddLast(new Vector2(28, 315));
            positions.AddLast(new Vector2(28, 340));
            positions.AddLast(new Vector2(53, 326));
            LinkedListNode<Vector2> current = positions.First;
            
            
            foreach (Player p in pm.getPlayerList())
            {
                p.GenerateAgent(current.Value, new Vector2(24, 24), new Vector2(12, 12), true, this);
                current = current.Next;
            }
        }

        private void respawnPlayer() //needs a new home
        {
            LinkedList<Vector2> positions = new LinkedList<Vector2>();
            positions.AddLast(new Vector2(3, 315));
            positions.AddLast(new Vector2(3, 340));
            positions.AddLast(new Vector2(28, 315));
            positions.AddLast(new Vector2(28, 340));
            positions.AddLast(new Vector2(53, 326));
            LinkedListNode<Vector2> current = positions.First;


            foreach (Player p in pm.getPlayerList())
            {
                if ((p.getAgent() == null || p.getAgent().readyForRemoval()) && p.c.StartPressed())
                {
                    p.getStats().set("hp", p.getStats().get("maxHP"));
                    p.GenerateAgent(current.Value, new Vector2(24, 24), new Vector2(12, 12), true, this);
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
