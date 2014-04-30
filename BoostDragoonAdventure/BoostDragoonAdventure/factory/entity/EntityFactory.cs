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
using wickedcrush.entity.physics_entity.agent.trap;
using wickedcrush.entity.physics_entity.agent.attack.projectile;
using wickedcrush.entity.physics_entity.agent.attack.melee;
using wickedcrush.entity.physics_entity.agent.enemy;

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

        public PlayerAgent addPlayerAgent(Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls c)
        {
            PlayerAgent agent = new PlayerAgent(w, pos, size, center, solid, c, this);
            em.addEntity(agent);
            
            return agent;
        }

        public void addAgent(Vector2 pos, Vector2 size, Vector2 center, bool solid, PersistedStats stats)
        {
            Murderer a = new Murderer(w, pos, size, center, solid, this, stats);
            if (map != null)
            {
                a.activateNavigator(map);
                if(pm.getPlayerList().Count > 0)
                    a.setTarget(pm.getPlayerList()[0].getAgent());
            }
            em.addEntity(a);
        }

        public void addTurret(Vector2 pos, Direction facing)
        {
            Turret t = new Turret(w, pos, this, facing);
            em.addEntity(t);
        }

        public void addBolt(Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force)
        {
            Bolt b = new Bolt(w, pos, size, center, parent, damage, force);
            em.addEntity(b);
        }

        public void addMeleeAttack(Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force)
        {
            MeleeAttack a = new MeleeAttack(w, pos, size, center, parent, damage, force);
            em.addEntity(a);
        }

        public void spawnPlayers()
        {
            foreach (Player p in pm.getPlayerList())
            {
                p.GenerateAgent(new Vector2(12, 320), new Vector2(24, 24), new Vector2(12, 12), true, this);
            }
        }

        private void respawnPlayer() //needs a new home
        {
            foreach (Player p in pm.getPlayerList())
            {
                if ((p.getAgent() == null || p.getAgent().readyForRemoval()) && p.c.StartPressed())
                {
                    p.GenerateAgent(new Vector2(12, 320), new Vector2(24, 24), new Vector2(12, 12), true, this);
                }
            }
        }

        public void Update() //debug,something else should have this loop
        {
            respawnPlayer();
            
        }
    }
}
