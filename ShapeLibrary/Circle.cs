using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ShapeLibraryTests")]
namespace ShapeLibrary
{
    internal class Circle : ICircle
    {
        //---Properties--------------------------------------------------------------
        public float Radius { get; }

        public Vector Center { get; }

        public Colour Colour { get; }

        private List<Vector> _vertices;

        //Teacher's suggestion: a constant for point count
        private const int POINT_COUNT = 60;
        //--------------------------------------------------------------------------

        //---Constructor--------------------------------------------------------------
        //After discussion with teacher I decided to make a circle with 60 points (6 points in the prep lab * 10)
        public Circle(float x, float y, float radius, Colour colour)
        {
            if (radius <= 0) 
            { 
                throw new ArgumentOutOfRangeException("Radius cannot be zero or less");
            }

            Radius = radius;
            Center = new Vector(x, y);
            Colour = colour;

            _vertices = null; //lazy load
        }
        //--------------------------------------------------------------------------

        public List<Vector> Vertices
        {
            get
            {
                if (_vertices == null)
                {
                    _vertices = CalculateVertices(Center.x, Center.y, Radius, POINT_COUNT);
                }
                return _vertices;
            }
        }

        //---Methods--------------------------------------------------------------
        //Changed logic from my prep lab
        //(xc, xy) = center of the circle, r = radius, n = num of points
        private List<Vector> CalculateVertices(float xc, float yc, float r, int n)
        {
            //init list of vertices
            var vertices = new List<Vector>(n);

            //"divide the circle into equal angles"
            //"Cannot implicitly convert type 'double' to 'float'" - add (float) for that
            float q = (float)(2 * Math.PI / n);

            //calculate the coordinates for each point
            for (int i = 0; i < n; i++)
            {
                float angle = i * q;
                float x = xc + r * (float)Math.Cos(angle);
                float y = yc + r * (float)Math.Sin(angle);
                vertices.Add(new Vector(x, y));
            }

            return vertices;
        }
        //-------------------------------------------------------------------------
    }
}