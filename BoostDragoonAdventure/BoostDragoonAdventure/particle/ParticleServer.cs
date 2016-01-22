using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.particle
{
    public static class ParticleServer
    {
        public static ParticleStruct GenerateParticle()
        {
            return new ParticleStruct(
                Vector3.Zero, Vector3.Zero,
                new Vector3(-0.3f, -0.3f, -0.3f), new Vector3(0.6f, 0.6f, 0.6f),
                new Vector3(0f, -0.03f, 0f),
                0f, 0f,
                100, 42,
                "particles", 0, "white_to_blue");
        }

        public static ParticleStruct GenerateSpark(Vector3 pos, Vector2 direction)
        {
            return new ParticleStruct(
                pos, Vector3.Zero,
                new Vector3(-direction.X / 2f, -0.3f, -direction.Y / 2f), new Vector3(direction.X, 0.6f, direction.Y),
                new Vector3(0f, -0.03f, 0f),
                0f, 0f,
                300, 32,
                "particles", 0, "white_to_yellow");

        }

        public static ParticleStruct GenerateSmoke(Vector3 pos)
        {
            return new ParticleStruct(
                pos, Vector3.Zero,
                new Vector3(-0.3f, -0.3f, -0.3f), new Vector3(0.6f, 1.6f, 0.6f),
                new Vector3(0f, -0.03f, 0f),
                0f, 0f,
                500, 32,
                "particles", 0, "white_to_blue");

        }
    }
}
