using ShapeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FireworksTest")]
namespace Fireworks
{
    internal class Firework : IFirework
    {
        //---FIELDS and PROPERTIES------------------------------------
        private readonly int _width;
        private readonly int _height;
        private readonly Colour _colour;
        private readonly IExplosionPattern _pattern;
        private readonly Random _rng;
        private bool _exploded;
        private IParticle _launcher;
        private readonly List<IParticle> _particles;

        //Random ranges consts for launcher lifespan 
        private const int LAUNCHER_LIFE_MIN = 45;
        private const int LAUNCHER_LIFE_MAX = 70;

        //Lifespan const for a particle
        private const int LIFESPAN = 120;

        private const int PARTICLE_PARALLEL_THRESHOLD = 200;

        //public properties
        public bool Exploded { get { return _exploded; } }

        public IParticle Launcher { get { return _launcher; } }

        public List<IParticle> Particles { get { return _particles; } }
        //--------------------------------------------------------------

        //---CONSTRUCTORS-----------------------------------------------
        //C1: random x and random lifespan
        public Firework(int width, int height, Colour colour, IExplosionPattern pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException("Screen width or height must be bigger than 0");
            }

            _width = width;
            _height = height;
            _colour = colour;
            _pattern = pattern;
            _particles = new List<IParticle>();
            _rng = new Random();
            _exploded = false;
            _launcher = null;
        }

        //C2: explicit x, y, lifespan
        public Firework(int width, int height, float x, float y, Colour colour, int lifespan, IExplosionPattern pattern) : this(width, height, colour, pattern)
        {
            //added additional validation after received feedback
            if (lifespan <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lifespan), "Lifespan must be bigger than zero!!");
            }

            if (x < 0 || x > width || y < 0 || y > height)
            { 
                throw new ArgumentOutOfRangeException("Launcher position must be inside the screen bounds!");
            }
            
            CreateLauncher(x, y, lifespan);
        }
        //-----------------------------------------------------------------

        //---INTERFACE METHODS---------------------------------------------
        public void Launch()
        {
            if (_launcher != null)
            {
                return; //because that means it's already launched
            }

            int lifespan = _rng.Next(LAUNCHER_LIFE_MIN, LAUNCHER_LIFE_MAX + 1);
            float x = (float)_rng.NextDouble() * _width;
            float y = _height;

            CreateLauncher(x, y, lifespan);
        }

        public void Update()
        {
            //if not laucnhed yet (1st cons) then nothing to do yet
            if (_launcher == null && !_exploded)
            {
                return;
            }

            //update launcher until out of lifespan
            if (!_exploded && _launcher != null)
            {
                _launcher.Update();

                if (_launcher.Done)
                {
                    Explode();
                }
            }

            if (_exploded && _particles.Count > 0)
            {
                int count = _particles.Count;

                // Update pass (parallel for large fireworks, sequential for small ones)
                if (count > PARTICLE_PARALLEL_THRESHOLD)
                {
                    Parallel.For(0, count, i =>
                    {
                        _particles[i].Update();
                    });
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        _particles[i].Update();
                    }
                }

                //Removal pass — sequential, safe to modify the list here
                _particles.RemoveAll(p => p.Done);
            }
        }
        //------------------------------------------------------------------

        //---HELPER METHODS-------------------------------------------------
        private void CreateLauncher(float x, float y, int lifespan)
        {
            _launcher = ParticleFactory.Create(x, y, _colour, lifespan);

            //pattern launch velocity only affects Y
            _launcher.ApplyVelocity(_pattern.LaunchVelocity);
        }
        private void Explode()
        {
            _exploded = true;

            Vector basePosition = _launcher.Position;
            Vector baseVelocity = _launcher.Velocity;

            int count = _pattern.NumberOfParticles;

            for (int i = 0; i < count; i++)
            {
                //each particle is launcher's last velocity + pattern explosion velocity
                Vector initialVelocity = baseVelocity + _pattern.ExplosionVelocity;

                IParticle p = ParticleFactory.Create(basePosition.x, basePosition.y, _colour, LIFESPAN);
                p.ApplyVelocity(initialVelocity);

                _particles.Add(p);
            }

            //launcher no longer needed after explosion
            _launcher = null;
        }
        //------------------------------------------------------------------
    }
}
