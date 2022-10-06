internal class SvgPlotter
{
    public SvgPlotter(PlotterSettings settings)
    {
        _circles = new List<string>();
        _lines = new List<string>();
        _text = new List<string>();
        _systems = new List<PlanetInfo>();

        _scale = settings.Scale;

        _width = (int)Math.Ceiling(settings.Width * _scale);
        _height = (int)Math.Ceiling(settings.Height * _scale);

        _centerX = _width / 2.0;
        _centerY = _height / 2.0;

        _transformX = settings.Center.X;
        _transformY = settings.Center.Y;

        _systemRadius = settings.SystemRadius;
        _systemPalette = settings.SystemPalette;
        _subtitleMapping = settings.SystemSubtitleMapping;
        _linkPalette = settings.LinkPalette;
        _linkStrokeWidth = settings.LinkStrokeWidth;

        _importantMapping = settings.ImportantWorldMapping;

        _includeJumpLines = settings.IncludeJumpLines;
        _includeSystemNames = settings.IncludeSystemNames;

        _primaryFontSize = settings.PrimaryFontSize;
        _secondaryFontSize = settings.SecondaryFontSize;
    }

    public void Add(PlanetInfo system)
    {
        (double transformedX, double transformedY) = TransformCoordinates(system.Coordinates);

        var color = "#ffffff";
        if (_systemPalette != null)
        {
            color = _systemPalette(system);
        }

        double overshoot = 30 * _scale;
        if (transformedX > 0 && transformedX < _width && transformedY > 0 && transformedY < _height)
        {
            bool systemIsImportant = false;
            if (_importantMapping != null)
            {
                systemIsImportant = _importantMapping(system);
            }

            var radius = systemIsImportant ? _systemRadius * 2 : _systemRadius;
            string svg = $"<circle cx=\"{transformedX}\" cy=\"{transformedY}\" r=\"{radius}\" stroke=\"#000000\" stroke-width=\"1\" fill=\"{color}\" />";
            _circles.Add(svg);

            if (_includeSystemNames)
            {
                int mainFontSize = _primaryFontSize;
                int subFontSize = _secondaryFontSize;
                string label = $"<text x=\"{transformedX}\" y=\"{transformedY - (radius + subFontSize + 4)}\" fill=\"#eeeeee\" text-anchor=\"middle\" font-family=\"sans-serif\" font-size=\"{mainFontSize}\" stroke=\"black\" stroke-width=\"0.25\">{system.Name}</text>";
                _text.Add(label);

                if (_subtitleMapping != null)
                {
                    string subtitle = _subtitleMapping(system);
                    string subtitleElement = $"<text x=\"{transformedX}\" y=\"{transformedY - (radius + 2)}\" fill=\"#eeeeee\" text-anchor=\"middle\" font-family=\"sans-serif\" font-size=\"{subFontSize}\" stroke=\"black\" stroke-width=\"0.25\">{subtitle}</text>";
                    _text.Add(subtitleElement);
                }
                
            }

            _systems.Add(system);
        }
        else if (transformedX > (0 - overshoot) && transformedX < (_width + overshoot) && transformedY > (0 - overshoot) && transformedY < (_height + overshoot))
        {
            _systems.Add(system);
        }
    }

    public void Write(string file)
    {
        if (_includeJumpLines)
        {
            GenerateLines();
        }
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
        _lines = new List<string>();
        for (int i = 0; i < _systems.Count; i++)
        {
            var system1 = _systems[i];
            (double x1, double y1) = TransformCoordinates(system1.Coordinates);

            for (int j = i + 1; j < _systems.Count; j++)
            {
                var system2 = _systems[j];
                if (IsSingleJump(system1, system2))
                {
                    (double x2, double y2) = TransformCoordinates(system2.Coordinates);

                    string color = "#666666";
                    if (_linkPalette != null)
                    {
                        color = _linkPalette(system1, system2);
                    }
                    string svg = $"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" stroke=\"{color}\" stroke-width=\"{_linkStrokeWidth}\" opacity=\"0.3\" />";
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

    private (double, double) TransformCoordinates(SystemCoordinates coordinates)
    {
        double transformedX = (_scale * (coordinates.X - _transformX)) + _centerX;
        double transformedY = (-1 * _scale * (coordinates.Y - _transformY)) + _centerY;
        return (transformedX, transformedY);
    }

    private List<PlanetInfo> _systems;
    private List<string> _circles;
    private List<string> _lines;
    private List<string> _text;
    private int _width;
    private int _height;
    private double _centerX;
    private double _centerY;
    private double _transformX;
    private double _transformY;
    private double _scale;
    private SystemColorMapping? _systemPalette;
    private SystemSubtitleMapping? _subtitleMapping;
    private LinkColorMapping? _linkPalette;
    private ImportantWorldMapping? _importantMapping;
    private int _systemRadius;
    private bool _includeJumpLines;
    private bool _includeSystemNames;
    private int _primaryFontSize;
    private int _secondaryFontSize;
    private double _linkStrokeWidth;
}