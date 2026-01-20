namespace ShapeLibrary;

public struct Colour
{
    //---Properties-------------------------------------------------
    public int Red   { get; private set; }
    public int Green { get; private set; }
    public int Blue  { get; private set; }
    //--------------------------------------------------------------

    //---Constructor------------------------------------------------
    public Colour(int red, int green, int blue)
    {
        Red = Check(red);
        Green = Check(green);
        Blue  = Check(blue);
    }
    //--------------------------------------------------------------

    //---Overloading operators--------------------------------------
    //+ and - operators
    public static Colour operator +(Colour a, Colour b) =>
        new Colour(a.Red + b.Red, a.Green + b.Green, a.Blue + b.Blue);

    public static Colour operator -(Colour a, Colour b) =>
        new Colour(a.Red - b.Red, a.Green - b.Green, a.Blue - b.Blue);

    //multiply a Colour by an int (scale each component)
    public static Colour operator *(Colour c, int k) =>
        new Colour(c.Red * k, c.Green * k, c.Blue * k);

    public static Colour operator *(int k, Colour c)
    {
        return c * k;
    }
    //--------------------------------------------------------------

    //---Helper methods---------------------------------------------
    //keep values in [0,255]
    private static int Check(int v)
    {
        if (v < 0)
        {
            return 0;
        }
        if (v > 255)
        {
            return 255;
        }
        return v;
    }
    //--------------------------------------------------------------

    //---Equality methods-------------------------------------------
    //== and != to check equality of two Colour values
    public static bool operator ==(Colour left, Colour right) =>
        left.Red == right.Red && left.Green == right.Green && left.Blue == right.Blue;

    public static bool operator !=(Colour left, Colour right) => !(left == right);
    //--------------------------------------------------------------

    //string representation
    public override string ToString() => $"Colour(R:{Red}, G:{Green}, B:{Blue})";
}