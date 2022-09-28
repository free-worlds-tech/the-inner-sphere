internal class SvgPlotter
{
    public SvgPlotter(PlotterSettings settings)
    {
        _circles = new List<string>();
        _lines = new List<string>();
        _text = new List<string>();
        _systems = new List<PlanetInfo>();

        _scale = 10;

        _width = (int)Math.Ceiling(settings.Width * _scale);
        _height = (int)Math.Ceiling(settings.Height * _scale);

        _centerX = _width / 2.0;
        _centerY = _height / 2.0;

        _palette = settings.Palette;
    }

    public void Add(PlanetInfo system)
    {
        double transformedX = (_scale * system.Coordinates.X) + _centerX;
        double transformedY = (-1 * _scale * system.Coordinates.Y) + _centerY;

        var color = "#ffffff";
        if (_palette != null)
        {
            color = _palette(system);
        }

        if (transformedX > 0 && transformedX < _width && transformedY > 0 && transformedY < _height)
        {
            string svg = $"<circle cx=\"{transformedX}\" cy=\"{transformedY}\" r=\"12\" stroke=\"#000000\" stroke-width=\"1\" fill=\"{color}\" />";
            _circles.Add(svg);

            string label = $"<text x=\"{transformedX}\" y=\"{transformedY - 16}\" fill=\"#eeeeee\" text-anchor=\"middle\" font-family=\"sans-serif\" font-size=\"16\" stroke=\"black\" stroke-width=\"0.25\">{system.Name}</text>";
            _text.Add(label);

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
            foreach (var text in _text)
            {
                writer.WriteLine($"  {text}");
            }
            writer.WriteLine("</svg>");
        }
    }

    private void GenerateLines()
    {
        for (int i = 0; i < _systems.Count; i++)
        {
            var system1 = _systems[i];
            double x1 = (_scale * system1.Coordinates.X) + _centerX;
            double y1 = (-1 * _scale * system1.Coordinates.Y) + _centerY;

            for (int j = i + 1; j < _systems.Count; j++)
            {
                var system2 = _systems[j];
                if (IsSingleJump(system1, system2))
                {
                    double x2 = (_scale * system2.Coordinates.X) + _centerX;
                    double y2 = (-1 * _scale * system2.Coordinates.Y) + _centerY;
                    string svg = $"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" stroke=\"#666666\" stroke-width=\"1\" />";
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
    private List<string> _text;
    private int _width;
    private int _height;
    private double _centerX;
    private double _centerY;
    private double _scale;
    private ColorMapping? _palette;
}