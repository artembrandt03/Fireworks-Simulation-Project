using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeLibrary
{
    public static class ShapesFactory
    {
        //Creating Circle with 60 points
        public static ICircle CreateCircle(float x, float y, float radius, Colour colour)
        {
            return new Circle(x, y, radius, colour);
        }

        //Creating Rectangle
        public static IRectangle CreateRectangle(float x, float y, float width, float height, Colour colour)
        {
            return new Rectangle(x, y, width, height, colour); 
        }

        public static IShape CreateStar(float x, float y, float radius, Colour colour)
        {
            return new StarShape(x, y, radius, colour);
        }
    }
}

