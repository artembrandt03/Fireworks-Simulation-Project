using ShapeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fireworks
{
    internal class ShapeExplosionPattern : IExplosionPattern
    {
        //---FIELDS and PROPERTIES------------------------------------
        private static readonly Random _rng = new Random();
        private readonly List<Vector> _dirs; //unit directions from center to each vertex
        public int NumberOfParticles { get; }
        public Vector LaunchVelocity { get; }

        //Logic: each call tot his returns next direction * random speed + tiny jitter to make it imperfect
        public Vector ExplosionVelocity
        {
            get
            {
                //logic: pop next direction from list because we want each particle to go in different direction
                Vector baseDir;
                if (_dirs.Count > 0)
                {
                    baseDir = _dirs[_dirs.Count - 1];
                    _dirs.RemoveAt(_dirs.Count - 1); 
                }
                else
                {
                    //Fallback: if somehow called extra times, pick any simple direction
                    baseDir = new Vector(1f, 0f);
                }

                //Refactored small jitter
                float jx = (float)(_rng.NextDouble() * 0.25 - 0.125f);
                float jy = (float)(_rng.NextDouble() * 0.25 - 0.125f);
                var dir = new Vector(baseDir.x + jx, baseDir.y + jy);

                //Normalizing
                float m = Vector.Magnitude(dir);
                if (m > 0f) dir = new Vector(dir.x / m, dir.y / m);

                //Random speed
                float speed = 5.0f + (float)_rng.NextDouble() * 1.5f;
                return dir * speed;
            }
        }
        //-------------------------------------------------------------

        //---CONSTRUCTOR----------------------------------------------
        public ShapeExplosionPattern(IShape shape)
        {
            //Validation
            if (shape == null) throw new ArgumentNullException(nameof(shape));
            if (shape.Vertices == null || shape.Vertices.Count == 0) throw new ArgumentException("Shape has no vertices!!!", nameof(shape));

            //Decided to add center point of a shape (average of all vertex positions)
            //When we explode the shape, particles will go from center to each vertex
            float cx = 0, cy = 0;
            foreach (var v in shape.Vertices)
            {
                cx += v.x;
                cy += v.y;
            }
            cx /= shape.Vertices.Count;
            cy /= shape.Vertices.Count;

            //Calculate unit directions from center to each vertex
            var allDirs = new List<Vector>(shape.Vertices.Count);
            foreach (var v in shape.Vertices)
            {
                var d = new Vector(v.x - cx, v.y - cy);
                float m = Vector.Magnitude(d);
                if (m > 0f) allDirs.Add(new Vector(d.x / m, d.y / m));
            }
            if (allDirs.Count == 0) allDirs.Add(new Vector(1f, 0f));

            //Addition/Refactoring to have rectangle with 4 vertices and circle with 8
            List<Vector> chosen = new List<Vector>();

            if (allDirs.Count == 4)
            {
                //Rectangle - use the 4 corners with one particle per corner
                chosen.AddRange(allDirs);
            }
            else if (allDirs.Count >= 8)
            {
                //Circle - 8 evenly ish spaced directions
                int n = allDirs.Count;
                for (int k = 0; k < 8; k++)
                {
                    int idx = (k * n) / 8;   
                    if (idx >= n) idx = n - 1;       
                    //Avoid duplicates
                    if (chosen.Count == 0 || !Same(allDirs[idx], chosen[chosen.Count - 1]))
                        chosen.Add(allDirs[idx]);
                }
            }
            else
            {
                //For other shapes in future: just use what they have
                chosen.AddRange(allDirs);
            }

            _dirs = chosen; //we’ll pop from this list in ExplosionVelocity
            NumberOfParticles = _dirs.Count;

            //Upward launch before exploding
            float vy = -(6.0f + (float)_rng.NextDouble() * 2.0f);
            LaunchVelocity = new Vector(0f, vy);
        }

        private static bool Same(Vector a, Vector b)
        {
            //Simple equality with a tiny tolerance (delta) (to avoid duplicate picks on very small shapes)
            const float eps = 1e-4f;
            return Math.Abs(a.x - b.x) < eps && Math.Abs(a.y - b.y) < eps;
        }
    }
}
