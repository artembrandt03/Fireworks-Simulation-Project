using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapeLibrary;

namespace Fireworks
{
    public static class ParticleFactory
    {
        public static IParticle Create(float x, float y, Colour colour, int lifespan)
        {
            return new Particle(x, y, colour, lifespan);
        }
    }
}
