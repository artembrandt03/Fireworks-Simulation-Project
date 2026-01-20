using ShapeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("FireworksTest")]

namespace Fireworks
{
    public static class ExplosionPatternFactory
    {
        public static IExplosionPattern Create()
        {
            return new ExplosionPattern();
        }

        public static IExplosionPattern Create(IShape shape)
        {
            return new ShapeExplosionPattern(shape);
        }

        public static IExplosionPattern CreateRectanglePattern(float x, float y, float w, float h, Colour colour)
        {
            IRectangle rectangle = ShapesFactory.CreateRectangle(x, y, w, h, colour);
            return new ShapeExplosionPattern(rectangle);
        }

        public static IExplosionPattern CreateCirclePattern(float x, float y, float radius, Colour colour)
        {
            ICircle circle = ShapesFactory.CreateCircle(x, y, radius, colour);
            return new ShapeExplosionPattern(circle);
        }

        public static IExplosionPattern CreateStarPattern(float x, float y, float radius, Colour colour)
        {
            IShape star = ShapesFactory.CreateStar(x, y, radius, colour);
            return new ShapeExplosionPattern(star);
        }
    }
}