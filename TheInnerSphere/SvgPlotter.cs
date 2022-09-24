internal class SvgPlotter
{
    public SvgPlotter(PlotterSettings settings)
    {
        _circles = new List<string>();
        _lines = new List<string>();
        _systems = new List<PlanetInfo>();

        _width = settings.Width * 1;
        _height = settings.Height * 1;

        _centerX = _width / 2.0;
        _centerY = _height / 2.0;

        _palette = settings.Palette;
    }

    public void Add(PlanetInfo system)
    {
        double transformedX = (1 * system.Coordinates.X) + _centerX;
        double transformedY = (-1 * system.Coordinates.Y) + _centerY;

        var color = "#ffffff";
        if (_palette != null)
        {
            color = _palette(system);
        }

        if (transformedX > 0 && transformedX < _width && transformedY > 0 && transformedY < _height)
        {
            string svg = $"<circle cx=\"{transformedX}\" cy=\"{transformedY}\" r=\"3\" stroke=\"black\" stroke-width=\"0.5\" fill=\"{color}\" />";
            _circles.Add(svg);

            _systems.Add(system);
        }
    }

    public void Write(string file)
    {
        GenerateLines();
        using (var writer = new StreamWriter(file))
        {
            writer.WriteLine("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");
            writer.WriteLine($"<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" height=\"{_height}\" width=\"{_width}\" >");
            writer.WriteLine($"  <rect height=\"{_height}\" width=\"{_width}\" fill=\"#000000\" />");
            foreach (var line in _lines)
            {
                writer.WriteLine($"  {line}");
            }
            foreach (var circle in _circles)
            {
                writer.WriteLine($"  {circle}");
            }
            writer.WriteLine("</svg>");
        }
    }

    private void GenerateLines()
    {
        for (int i = 0; i < _systems.Count; i++)
        {
            for (int j = i + 1; j < _systems.Count; j++)
            {
                var system1 = _systems[i];
                var system2 = _systems[j];
                if (IsSingleJump(system1, system2))
                {
                    double x1 = (1 * system1.Coordinates.X) + _centerX;
                    double y1 = (-1 * system1.Coordinates.Y) + _centerY;
                    double x2 = (1 * system2.Coordinates.X) + _centerX;
                    double y2 = (-1 * system2.Coordinates.Y) + _centerY;
                    string svg = $"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" stroke=\"#cccccc\" stroke-width=\"0.25\" />";
                    _lines.Add(svg);
                }
            }
        }
    }

    private bool IsSingleJump(PlanetInfo a, PlanetInfo b)
    {
        double distanceSquared = Math.Pow(a.Coordinates.X - b.Coordinates.X,2) + Math.Pow(a.Coordinates.Y - b.Coordinates.Y, 2);
        return (distanceSquared <= 900);
    }

    private List<PlanetInfo> _systems;
    private List<string> _circles;
    private List<string> _lines;
    private int _width;
    private int _height;
    private double _centerX;
    private double _centerY;
    private ColorMapping? _palette;
}