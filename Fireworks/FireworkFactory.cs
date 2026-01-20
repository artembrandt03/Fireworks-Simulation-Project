using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using ShapeLibrary;

namespace Fireworks
{
    public static class FireworkFactory
    {
        //For C1: random x and lifespan
        public static IFirework Create(int width, int height, Colour colour, IExplosionPattern pattern)
        {
            return new Firework(width, height, colour, pattern);
        }

        //For C2: explicit x, y, lifespan
        public static IFirework Create(int width, int height, float x, float y, Colour colour, int lifespan, IExplosionPattern pattern)
        {
            return new Firework(width, height, x, y, colour, lifespan, pattern);
        }
    }
}
