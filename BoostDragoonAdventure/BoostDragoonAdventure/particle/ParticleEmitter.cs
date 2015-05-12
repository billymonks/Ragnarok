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
            for (int i = 0; i < count; i++)
            {
                Particle p = new Particle(ps, factory);

                if (Math.Abs((factory._gm.camera.cameraPosition.X + 320) - ps.pos.X) < (600 * factory._gm.camera.fov) && Math.Abs((factory._gm.camera.cameraPosition.Y + 240) - ps.pos.Z) < 480)
                    _pm.AddParticle(p);
            }
        }

    }
}
