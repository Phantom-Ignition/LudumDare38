using MonoGame.Extended.Particles.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Particles;

namespace LudumDare38.ParticleModifiers
{
    class VelocityModifier : IModifier
    {
        public float VelocityThreshold { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var velocityThreshold2 = VelocityThreshold * VelocityThreshold;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Velocity *= VelocityThreshold;
            }
        }
    }
}
