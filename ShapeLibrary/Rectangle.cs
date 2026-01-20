using ShapeLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ShapeLibraryTests")]
namespace ShapeLibrary
{
    internal class Rectangle : IRectangle
    {
        public float X { get; }

        public float Y { get; }

        public float Width { get; }

        public float Height { get; }

        private List<Vector> _vertices;

        public Colour Colour { get; }
        
        public Rectangle(float x, float y, float width, float height, Colour colour)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Colour = colour;
        }

        public List<Vector> Vertices
        {
            get
            {
                if (_vertices == null)
                {
                    _vertices = new List<Vector>
                    {
                        new Vector(X, Y),
                        new Vector(X + Width, Y),
                        new Vector(X + Width, Y + Height),
                        new Vector(X, Y + Height)
                    };
                }
                return _vertices;
            }
        }
    }
}
