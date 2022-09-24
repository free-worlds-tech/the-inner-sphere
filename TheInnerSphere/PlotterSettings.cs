internal delegate string ColorMapping(PlanetInfo system);

internal class PlotterSettings
{
    public PlotterSettings(int width, int height, ColorMapping? palette = null)
    {
        Width = width;
        Height = height;
        Palette = palette;
    }

    public int Width { get; }
    public int Height { get; }
    public ColorMapping? Palette { get; }
}