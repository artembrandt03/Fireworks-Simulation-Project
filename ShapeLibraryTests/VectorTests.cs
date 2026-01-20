using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeLibrary;

namespace ShapeLibraryTests
{
    //Unit tests for the Vector struct in ShapeLibrary
    [TestClass]
    public class VectorTests
    {
        //1) Constructor + copy constructor test
        [TestMethod]
        public void Constructor_And_CopyConstructor()
        {
            var v = new Vector(3f, -2f);
            Assert.AreEqual(3f, v.x);
            Assert.AreEqual(-2f, v.y);

            var c = new Vector(v);
            Assert.AreEqual(3f, c.x);
            Assert.AreEqual(-2f, c.y);
        }

        //2) Addition and subtraction test
        [TestMethod]
        public void Add_And_Subtract()
        {
            var a = new Vector(1f, 5f);
            var b = new Vector(2f, -3f);

            var sum = a + b;   //should be (3, 2)
            Assert.AreEqual(3f, sum.x);
            Assert.AreEqual(2f, sum.y);

            var diff = a - b;  //should be (-1, 8)
            Assert.AreEqual(-1f, diff.x);
            Assert.AreEqual(8f, diff.y);
        }

        // 3)Scalar multiplication and division
        [TestMethod]
        public void Scalar_Multiply_And_Divide()
        {
            var v = new Vector(10f, -4f);

            var m = v * 2;  //(20, -8)
            Assert.AreEqual(20f, m.x);
            Assert.AreEqual(-8f, m.y);

            var d = v / 2;  //(5, -2)
            Assert.AreEqual(5f, d.x);
            Assert.AreEqual(-2f, d.y);
        }

        // 4)Magnitude and Normalize test (with small delta for float comparisons)
        [TestMethod]
        public void Magnitude_And_Normalize()
        {
            var v = new Vector(3f, 4f);

            var mag = Vector.Magnitude(v);      //should be 5
            Assert.AreEqual(5f, mag, 0.0001f);

            var n = Vector.Normalize(v);        //(0.6, 0.8)
            Assert.AreEqual(0.6f, n.x, 0.0001f);
            Assert.AreEqual(0.8f, n.y, 0.0001f);
            Assert.AreEqual(1f, Vector.Magnitude(n), 0.0001f);

            //zero vector will stay zero after Normalize since we have a check for that
            var nz = Vector.Normalize(new Vector(0f, 0f));
            Assert.AreEqual(0f, nz.x);
            Assert.AreEqual(0f, nz.y);
        }

        //---------- Additional unit tests post assignment feedback ----------
        [TestMethod]
        public void Scalar_Multiply_Int_And_Float_Both_Orders()
        {
            var v = new Vector(10f, -4f);

            //int
            var m1 = v * 2;          
            var m2 = 2 * v;          //switch around
            Assert.AreEqual(20f, m1.x); Assert.AreEqual(-8f, m1.y);
            Assert.AreEqual(m1.x, m2.x); Assert.AreEqual(m1.y, m2.y);

            //float
            var mf = v * 0.5f;      
            var mf2 = 0.5f * v;      //switch around
            Assert.AreEqual(5f, mf.x, 1e-4);
            Assert.AreEqual(-2f, mf.y, 1e-4);
            Assert.AreEqual(mf.x, mf2.x, 1e-4);
            Assert.AreEqual(mf.y, mf2.y, 1e-4);

            //multiplication by zero
            var mz = v * 0f;
            Assert.AreEqual(0f, mz.x);
            Assert.AreEqual(0f, mz.y);
        }

        [TestMethod]
        public void Scalar_Divide_Int_And_Float()
        {
            var v = new Vector(10f, -4f);

            //int
            var d1 = v / 2;          
            Assert.AreEqual(5f, d1.x);
            Assert.AreEqual(-2f, d1.y);

            //float
            var d2 = v / 2.5f;       
            Assert.AreEqual(4f, d2.x, 1e-4);
            Assert.AreEqual(-1.6f, d2.y, 1e-4);

            //zero vector divided by non-zero scalar should remain zero!
            var z = new Vector(0f, 0f) / 3f;
            Assert.AreEqual(0f, z.x);
            Assert.AreEqual(0f, z.y);
        }

        [TestMethod]
        public void Divide_By_Zero_Throws()
        {
            var v = new Vector(3f, 4f);

            Assert.ThrowsException<DivideByZeroException>(() => { var _ = v / 0; });
            Assert.ThrowsException<DivideByZeroException>(() => { var _ = v / 0f; });
        }

        [TestMethod]
        public void Magnitude_And_Normalize_Including_Zero_Vector()
        {
            var v = new Vector(3f, 4f);
            var mag = Vector.Magnitude(v);
            Assert.AreEqual(5f, mag, 1e-4);

            var n = Vector.Normalize(v);
            Assert.AreEqual(0.6f, n.x, 1e-4);
            Assert.AreEqual(0.8f, n.y, 1e-4);
            Assert.AreEqual(1f, Vector.Magnitude(n), 1e-4);

            //zero vector cases
            var zero = new Vector(0f, 0f);
            Assert.AreEqual(0f, Vector.Magnitude(zero), 1e-6);
            var nz = Vector.Normalize(zero);
            Assert.AreEqual(0f, nz.x);
            Assert.AreEqual(0f, nz.y);
        }
    }
}

