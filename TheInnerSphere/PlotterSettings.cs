internal delegate string SystemColorMapping(PlanetInfo system);
internal delegate string LinkColorMapping(PlanetInfo system1, PlanetInfo system2);
internal delegate string SystemSubtitleMapping(PlanetInfo system);

internal class PlotterSettings
{
    public PlotterSettings()
    {
        Width = 1600;
        Height = 1600;
        SystemPalette = null;
        LinkPalette = null;
        Scale = 10;
        SystemRadius = 12;
        IncludeJumpLines = true;
        IncludeSystemNames = true;
        Center = new SystemCoordinates(0,0);
    }

    public int Width { get; set; }
    public int Height { get; set; }
    public SystemCoordinates Center { get; set; }
    public SystemColorMapping? SystemPalette { get; set; }
    public SystemSubtitleMapping? SystemSubtitleMapping { get; set; }
    public LinkColorMapping? LinkPalette { get; set; }
    public int Scale { get; set; }
    public int SystemRadius { get; set; }
    public bool IncludeJumpLines { get; set; }
    public bool IncludeSystemNames { get; set; }
}