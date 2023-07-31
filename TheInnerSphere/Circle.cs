internal class Circle
{
    public Circle()
    {
        Center = new SystemCoordinates(0,0);
        Radius = 0;
    }

    public Circle(SystemCoordinates center, double radius)
    {
        Center = center;
        Radius = radius;
    }

    public SystemCoordinates Center { get; set; }
    public double Radius { get; set; }
}