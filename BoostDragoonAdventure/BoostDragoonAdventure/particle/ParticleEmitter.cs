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
                _pm.AddParticle(p);
            }
        }

    }
}
