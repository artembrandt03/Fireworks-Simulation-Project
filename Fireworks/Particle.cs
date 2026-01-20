using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapeLibrary;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FireworksTest")]

namespace Fireworks
{
    internal class Particle : IParticle
    {
        //---Properties--------------------------------------------------------------
        public Vector Acceleration { get; private set; }

        public Vector Velocity { get; private set; }

        public Vector Position { get; private set; }

        public ICircle Circle { get { return _circle; } }

        public Colour Colour { get; private set; }

        public bool Done { get { return _lifespan <= 0; } }

        private ICircle _circle;
        private int _lifespan;

        private const float DEFAULT_RADIUS = 3.0f;
        private readonly float _radius;

        //constant gravity: positive Y pulls downward
        private static readonly Vector GRAVITY = new Vector(0f, 0.2f);
        //--------------------------------------------------------------------------

        //---Constructor--------------------------------------------------------------
        public Particle(float x, float y, Colour colour, int lifespan)
        {
            if (lifespan <= 0)
            {
                throw new ArgumentOutOfRangeException("Lifespan must be bigger than 0.");
            }

            Position = new Vector(x, y);
            Velocity = new Vector(0f, 0f);
            Acceleration = new Vector(0f, 0f);
            Colour = colour;
            _lifespan = lifespan;
            _radius = DEFAULT_RADIUS;

            _circle = ShapeLibrary.ShapesFactory.CreateCircle(Position.x, Position.y, _radius, Colour);
        }
        //--------------------------------------------------------------------------

        //---Methods--------------------------------------------------------------
        public void ApplyGravity()
        {
            Acceleration = GRAVITY;
        }

        public void ApplyVelocity(Vector velocity)
        {
            Velocity = Velocity + velocity;
        }

        public void Update()
        {
            //if lifespan is 0 or less do nothing
            if (Done)
            {
                return;
            }
            //Apply gravity automatically once per frame
            Velocity = Velocity + new Vector(0f, 0.1f);
            Position = Position + Velocity;
            _lifespan--;
            _circle = ShapesFactory.CreateCircle(Position.x, Position.y, _radius, Colour);
        }
        //--------------------------------------------------------------------------
    }
}
