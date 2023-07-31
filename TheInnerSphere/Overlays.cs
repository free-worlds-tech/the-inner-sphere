internal class Overlays
{
    public Overlays()
    {
        Rectangles = new List<Rectangle>();
        Circles = new List<Circle>();
    }

    public List<Rectangle> Rectangles { get; set; }
    public List<Circle> Circles { get; set; }
}