using ShapeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeLibraryTests
{
    [TestClass]
    public class CircleTests
    {

        //Basic Constructor unit test
        [TestMethod]
        public void Circle_InitializesCorrectly()
        {
            //Arrange
            float x = 0f;
            float y = 0f;
            float radius = 5f;
            Colour colour = new Colour(255, 0, 0);

            //Act
            Circle circle = new Circle(x, y, radius, colour);

            //Assert
            Assert.AreEqual(radius, circle.Radius);
            Assert.AreEqual(x, circle.Center.x);
            Assert.AreEqual(y, circle.Center.y);
            Assert.AreEqual(colour, circle.Colour);
        }

        //Constructor zero radius validation check
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Circle_Throws_IfRadiusIsZero()
        {
            //Arrange
            var colour = new Colour(0, 0, 0);

            //Act & Assert (should throw)
            var circle = new Circle(0, 0, 0, colour);
        }

        //Constructor negative radius validation check 
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Circle_Throws_IfRadiusIsNegative()
        {
            //Arrange
            var colour = new Colour(0, 0, 0);

            //Act & Assert (should throw)
            var circle = new Circle(0, 0, -10, colour);
        }

        //Testing lazy loading - Vertices list should only be created once 
        [TestMethod]
        public void Vertices_AreLazyLoaded()
        {
            var circle = new Circle(10f, 20f, 15f, new Colour(10, 20, 30));

            var v1 = circle.Vertices;
            var v2 = circle.Vertices;

            Assert.AreSame(v1, v2);
            Assert.IsTrue(v1.Count > 0);
        }

        //The circle should always have 60 points
        [TestMethod]
        public void Vertices_Count_Is60()
        {
            var circle = new Circle(0f, 0f, 25f, new Colour(1, 2, 3));
            var v = circle.Vertices;

            Assert.AreEqual(60, v.Count);
        }

        //Colour should stay the same 
        [TestMethod]
        public void Circle_ColourStored()
        {
            var colour = new Colour(200, 150, 100);
            var circle = new Circle(1f, 2f, 3f, colour);

            Assert.AreEqual(colour, circle.Colour);
        }

    }
}
