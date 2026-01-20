using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeLibrary
{
    public struct Vector
    {
        //---Properties-------------------------------------------------
        public float x { get; private set; }
        public float y { get; private set; }
        //--------------------------------------------------------------

        //---Constructors------------------------------------------------
        public Vector(float X, float Y)
        {
            x = X;
            y = Y;
        }
        //Overloading constructor
        public Vector(Vector v)
        {
            x = v.x;
            y = v.y;
        }
        //--------------------------------------------------------------

        //---Overloading operators--------------------------------------
        public static Vector operator +(Vector a, Vector b) 
        {
            return new Vector(a.x + b.x, a.y + b.y);
        }
        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.x - b.x, a.y - b.y);
        }
        public static Vector operator *(Vector a, int scalar)
        {
            return new Vector(a.x * scalar, a.y * scalar);
        }
        public static Vector operator *(int scalar, Vector a)
        {
            return a * scalar;
        }
        public static Vector operator *(Vector a, float scalar)
        {
            return new Vector(a.x * scalar, a.y * scalar);
        }
        public static Vector operator *(float scalar, Vector a)
        {
            return a * scalar;
        }
        public static Vector operator /(Vector a, int scalar)
        {
            if (scalar == 0) throw new DivideByZeroException("Cannot divide a Vector by 0 (int).");
            return new Vector(a.x / scalar, a.y / scalar);
        }
        public static Vector operator /(Vector a, float scalar)
        {
            if (scalar == 0f) throw new DivideByZeroException("Cannot divide a Vector by 0 (float).");
            return new Vector(a.x / scalar, a.y / scalar);
        }
        //--------------------------------------------------------------

        //---Methods----------------------------------------------------
        public static float Magnitude(Vector v)
        {
            return (float)Math.Sqrt(v.x * v.x + v.y * v.y);
        }
        public static Vector Normalize(Vector v)
        {
            float m = Magnitude(v);
            if (m > 0)
            {
                return new Vector(v.x / m, v.y / m);
            }
            return new Vector(0, 0);
        }

        public override string ToString()
        {
            return $"(Vector: {x}, {y})";
        }


    }
}
