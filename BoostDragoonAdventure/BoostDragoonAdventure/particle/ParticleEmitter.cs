using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.particle;
using Microsoft.Xna.Framework;
using wickedcrush.utility;
using wickedcrush.factory.entity;

namespace wickedcrush.particle
{
    public class ParticleEmitter
    {
        private ParticleManager _pm;

        public ParticleEmitter(ParticleManager pm)
        {
            _pm = pm;
        }

        public void EmitParticles(ParticleStruct ps, EntityFactory factory, int count)
        {
            if (Math.Abs((factory._gm.camera.cameraPosition.X + 320) - ps.pos.X) < (600 * factory._gm.camera.fov) && Math.Abs((factory._gm.camera.cameraPosition.Y + 240) - ps.pos.Z) < 480)
            {
                for (int i = 0; i < count; i++)
                {
                    if (_pm.particlePool.Count == 0)
                    {
                        Particle p = new Particle(ps, factory);
                        _pm.AddParticle(p);
                    }
                    else
                    {
                        Particle p = _pm.particlePool.Pop();
                        p.Initialize(ps, factory);
                        _pm.AddParticle(p);
                    }
                }
            }
        }
    }
}
