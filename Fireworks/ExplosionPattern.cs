using System;
using ShapeLibrary;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FireworksTest")]

namespace Fireworks
{
    internal sealed class ExplosionPattern : IExplosionPattern
    {
        //---FIELDS and PROPERTIES------------------------------------
        private static readonly Random _rng = new Random();
        //Number of particles to spawn when it explodes
        private const int MIN_PARTICLES = 60;
        private const int MAX_PARTICLES = 100;
        //Explosion speed range (pixels per frame)
        private const float MIN_EXPLOSION_SPEED = 3.5f;
        private const float MAX_EXPLOSION_SPEED = 6.5f;
        //Upward launch speed range
        //Both will be negated (to go up)
        private const float MIN_LAUNCH_SPEED = 6.0f;
        private const float MAX_LAUNCH_SPEED = 9.0f;

        public int NumberOfParticles { get; private set; }
        public Vector ExplosionVelocity
        {
            get
            {
                float angle = RandomAngle();
                float speed = RandomRange(MIN_EXPLOSION_SPEED, MAX_EXPLOSION_SPEED);

                float vx = speed * (float)Math.Cos(angle);
                float vy = speed * (float)Math.Sin(angle);
                return new Vector(vx, vy);
            }
        }
        public Vector LaunchVelocity { get; private set; }
        //-------------------------------------------------------------

        //---CONSTRUCTOR----------------------------------------------
        public ExplosionPattern()
        {
            NumberOfParticles = _rng.Next(MIN_PARTICLES, MAX_PARTICLES + 1);

            float vy = RandomRange(MIN_LAUNCH_SPEED, MAX_LAUNCH_SPEED);
            LaunchVelocity = new Vector(0f, -vy);
        }
        //-------------------------------------------------------------

        //---HELPERS--------------------------------------------------
        //Returns a random float in [min, max]
        private static float RandomRange(float min, float max)
        {
            return min + (float)_rng.NextDouble() * (max - min);
        }

        //Returns a random angle in [0, 2π)
        private static float RandomAngle()
        {
            return (float)(2.0 * Math.PI * _rng.NextDouble());
        }
    }
}
