internal class Rectangle
{
    public Rectangle(SystemCoordinates a, SystemCoordinates b)
    {
        A = a;
        B = b;
    }

    public SystemCoordinates A { get; }
    public SystemCoordinates B { get; }
}