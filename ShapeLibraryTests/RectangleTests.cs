using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeLibrary;

namespace ShapeLibraryTests
{
    [TestClass]
    public class RectangleTests
    {
        /// <summary>
        /// Positive test: verifies that Vertices are calculated correctly
        /// for a standard rectangle (50,50) with width=100 and height=100.
        /// </summary>
        [TestMethod]
        public void Vertices_AreCalculatedCorrectly()
        {
            var rect = new Rectangle(50, 50, 100, 100, new Colour(255, 0, 0));
            var vertices = rect.Vertices;

            Assert.AreEqual(4, vertices.Count);
            Assert.AreEqual(50, vertices[0].x);
            Assert.AreEqual(50, vertices[0].y);
            Assert.AreEqual(150, vertices[1].x); 
            Assert.AreEqual(50, vertices[1].y);
            Assert.AreEqual(150, vertices[2].x); 
            Assert.AreEqual(150, vertices[2].y);
            Assert.AreEqual(50, vertices[3].x); 
            Assert.AreEqual(150, vertices[3].y);
        }

        /// <summary>
        /// Positive test: verifies that Vertices list is created lazily 
        /// and cached (same list reference returned).
        /// </summary>
        [TestMethod]
        public void Vertices_AreLazyLoaded()
        {
            var rect = new Rectangle(10, 10, 20, 20, new Colour(0, 255, 0));
            var v1 = rect.Vertices;
            var v2 = rect.Vertices;

            Assert.AreSame(v1, v2); 
        }

        /// <summary>
        /// Edge case: zero width rectangle. Some vertices overlap 
        /// but there should still be 4 vertices!
        /// </summary>
        [TestMethod]
        public void Rectangle_WithZeroWidth()
        {
            var rect = new Rectangle(10, 10, 0, 20, new Colour(111, 222, 111));
            var v = rect.Vertices;

            Assert.AreEqual(4, v.Count);
            Assert.AreEqual(new Vector(10, 10).ToString(), v[0].ToString());
            Assert.AreEqual(new Vector(10, 10).ToString(), v[1].ToString());
            Assert.AreEqual(new Vector(10, 30).ToString(), v[2].ToString());
            Assert.AreEqual(new Vector(10, 30).ToString(), v[3].ToString());
        }

        /// <summary>
        /// Edge case: rectangle with negative height. 
        /// </summary>
        [TestMethod]
        public void Rectangle_WithNegativeHeight()
        {
            var rect = new Rectangle(0, 0, 10, -10, new Colour(0, 0, 255));
            var v = rect.Vertices;

            Assert.AreEqual(4, v.Count);
            Assert.AreEqual(-10, v[2].y);
            Assert.AreEqual(-10, v[3].y);
        }
    }
}
