internal class SvgPlotter
{
    public SvgPlotter(int width, int height)
    {
        _circles = new List<string>();

        _width = width * 1;
        _height = height * 1;

        _centerX = _width / 2.0;
        _centerY = _height / 2.0;
    }

    public void Add(double x, double y, string color)
    {
        double transformedX = (1 * x) + _centerX;
        double transformedY = (-1 * y) + _centerY;

        string svg = $"<circle cx=\"{transformedX}\" cy=\"{transformedY}\" r=\"3\" stroke=\"black\" stroke-width=\"0.5\" fill=\"{color}\" />";
        _circles.Add(svg);
    }

    public void Write(string file)
    {
        using (var writer = new StreamWriter(file))
        {
            writer.WriteLine("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");
            writer.WriteLine($"<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" height=\"{_height}\" width=\"{_width}\" >");
            writer.WriteLine($"  <rect height=\"{_height}\" width=\"{_width}\" fill=\"#000000\" />");
            foreach (var circle in _circles)
            {
                writer.WriteLine($"  {circle}");
            }
            writer.WriteLine("</svg>");
        }
    }

    private List<string> _circles;
    private int _width;
    private int _height;
    private double _centerX;
    private double _centerY;
}