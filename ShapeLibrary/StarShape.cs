using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeLibrary
{
    internal class StarShape : IShape
    {
        public Colour Colour { get; }

        public List<Vector> Vertices { get; }

        public StarShape(float centerX, float centerY, float radius, Colour colour)
        {
            if (radius <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be bigger than 0!");
            }

            Colour = colour;
            Vertices = CreateStarVertices(centerX, centerY, radius);
        }

        private static List<Vector> CreateStarVertices(float cx, float cy, float r)
        {
            //Logic here: we just pick 5 equally spaced points on a circle around (cx, cy)
            //starting pointing up (angle = -90° = -PI/2).
            var vertices = new List<Vector>(5);

            float baseAngle = -(float)Math.PI / 2f;      //-90 degrees
            float step = 2f * (float)Math.PI / 5f;       //360 / 5 = 72 degrees

            for (int i = 0; i < 5; i++)
            {
                float angle = baseAngle + i * step;
                float x = cx + r * (float)Math.Cos(angle);
                float y = cy + r * (float)Math.Sin(angle);
                vertices.Add(new Vector(x, y));
            }

            return vertices;
        }
    }
}
