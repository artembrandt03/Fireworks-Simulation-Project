using ShapeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fireworks
{
    internal class StarExplosionPattern : IExplosionPattern
    {
        //---Field and Property Declarations---//
        private static readonly Random _rng = new Random();
        private readonly List<Vector> _directions;
        private const int STAR_PARTICLE_COUNT = 5;

        //Explosion speed range
        private const float MIN_EXPLOSION_SPEED = 3.5f;
        private const float MAX_EXPLOSION_SPEED = 6.5f;

        //Upward launch speed range
        private const float MIN_LAUNCH_SPEED = 6.0f;
        private const float MAX_LAUNCH_SPEED = 9.0f;

        public int NumberOfParticles { get; }

        public Vector LaunchVelocity { get; }

        //Each call to this will give one explosion velocity
        //which is direction + speed
        //(Logic similar to my other ExplosionPatterns)
        public Vector ExplosionVelocity
        {
            get
            {
                //Pick one of the star directions at random
                int index = _rng.Next(_directions.Count);
                Vector baseDir = _directions[index];

                //Tiny jitter
                float jitterX = RandomRange(-0.2f, 0.2f); //RandomRange will be implemented later
                float jitterY = RandomRange(-0.2f, 0.2f); //RandomRange will be implemented later
                Vector jittered = new Vector(baseDir.x + jitterX, baseDir.y + jitterY);

                // Re-normalize to unit length.
                float mag = Vector.Magnitude(jittered);
                if (mag > 0f)
                {
                    jittered = new Vector(jittered.x / mag, jittered.y / mag);
                }

                float speed = RandomRange(MIN_EXPLOSION_SPEED, MAX_EXPLOSION_SPEED); //RandomRange will be implemented later
                return jittered * speed;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------

        //--Constructor---------------------------------------------------------------------------------------------------------------
        public StarExplosionPattern(IShape shape)
        {
            //Quick validation
            if (shape == null) throw new ArgumentNullException(nameof(shape));
            if (shape.Vertices == null || shape.Vertices.Count == 0) throw new ArgumentException("Shape must have at least one vertex!!", nameof(shape));

            //Computing the center of the shape
            float cx = 0f;
            float cy = 0f;
            foreach (var v in shape.Vertices)
            {
                cx += v.x;
                cy += v.y;
            }
            cx /= shape.Vertices.Count;
            cy /= shape.Vertices.Count;

            //Building direction vectors from center to each vertex, normalized
            _directions = new List<Vector>();

            foreach (var v in shape.Vertices)
            {
                Vector dir = new Vector(v.x - cx, v.y - cy);
                float mag = Vector.Magnitude(dir);
                if (mag > 0f)
                {
                    _directions.Add(new Vector(dir.x / mag, dir.y / mag));
                }
            }

            //Fallback in weird case (not supposed to happen)
            //Will test this in unit tests
            if (_directions.Count == 0)
            {
                _directions.Add(new Vector(1f, 0f));
            }

            //We want exactly 5 particles (in case shape has more then 5 vertices)
            NumberOfParticles = Math.Min(STAR_PARTICLE_COUNT, _directions.Count);

            //Random upward launch velocity
            float vy = RandomRange(MIN_LAUNCH_SPEED, MAX_LAUNCH_SPEED); //RandomRange will be implemented later
            LaunchVelocity = new Vector(0f, -vy);
        }
        //---------------------------------------------------------------------------------------------------------------------------

        //---Private Methods----------------------------------------------------------------------------------------------------------
        //Simple helper method to get a random float in [min, max]
        private static float RandomRange(float min, float max)
        {
            return min + (float)_rng.NextDouble() * (max - min);
        }
    }
}
