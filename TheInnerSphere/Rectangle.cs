internal class Rectangle
{
    public Rectangle()
    {
        PointA = new SystemCoordinates(0,0);
        PointB = new SystemCoordinates(0,0);
    }

    public Rectangle(SystemCoordinates a, SystemCoordinates b)
    {
        PointA = a;
        PointB = b;
    }

    public SystemCoordinates PointA { get; set; }
    public SystemCoordinates PointB { get; set; }
}