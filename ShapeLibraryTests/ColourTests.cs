using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeLibrary;

namespace ShapeLibraryTests;

[TestClass]
public class ColourTests
{
    /// <summary>
    /// Positive/negative: checks that the constructor saturates inputs into [0,255].
    /// - Negative values become 0
    /// - Values above 255 become 255
    /// - In-range values stay the same
    /// </summary>
    [TestMethod]
    public void Ctor_ClampsValues_ToRange0_255()
    {
        var c = new Colour(-5, 128, 999);
        Assert.AreEqual(0, c.Red);
        Assert.AreEqual(128, c.Green);
        Assert.AreEqual(255, c.Blue);
    }

    /// <summary>
    /// multiplying by an integer saturates each component
    /// </summary>
    [TestMethod]
    public void Mul_Scales_And_Saturates()
    {
        var c = new Colour(130, 10, 2);

        var doubled = c * 2; //should saturate to 255 max
        Assert.AreEqual(255, doubled.Red);  // 260 -> 255
        Assert.AreEqual(20,  doubled.Green);
        Assert.AreEqual(4,   doubled.Blue);

        var zeroed = c * 0; //zeroes all components
        Assert.AreEqual(0, zeroed.Red);
        Assert.AreEqual(0, zeroed.Green);
        Assert.AreEqual(0, zeroed.Blue);

        var negative = c * -3; //negative scales saturate to 0
        Assert.AreEqual(0, negative.Red);
        Assert.AreEqual(0, negative.Green);
        Assert.AreEqual(0, negative.Blue);
    }

    /// <summary>
    /// tests custom equality operators.
    /// - Colours with identical RGB are equal
    /// - Colours with different rbg values are not equal
    /// </summary>
    [TestMethod]
    public void Equality_And_Inequality()
    {
        var x = new Colour(1, 2, 3);
        var y = new Colour(1, 2, 3);
        var z = new Colour(1, 2, 4);

        Assert.IsTrue(x == y);
        Assert.IsFalse(x != y);
        Assert.IsTrue(x != z);
        Assert.IsFalse(x == z);
    }

    /// <summary>
    /// tests that ToString() shows the RGB values
    /// </summary>
    [TestMethod]
    public void ToString_ShowsRgb()
    {
        var c = new Colour(12, 34, 56);
        var s = c.ToString();
        StringAssert.Contains(s, "R:12");
        StringAssert.Contains(s, "G:34");
        StringAssert.Contains(s, "B:56");
    }
}