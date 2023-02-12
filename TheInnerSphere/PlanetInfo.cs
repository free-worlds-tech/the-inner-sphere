using System.Text.RegularExpressions;

internal class PlanetInfo
{
    public PlanetInfo(uint id, string name, double x, double y, string owner, string ownershipNote)
    {
        Name = name;
        Id = id;
        Coordinates = new SystemCoordinates(x, y);
        Owners = new SimplePlanetFactionInfo(owner, ownershipNote);
        SarnaUrl = "";
    }

    public string Name { get; }
    public uint Id { get;  }
    public SystemCoordinates Coordinates { get; }
    public string SarnaUrl { get; }
    public IPlanetFactionInfo Owners { get; }

    public string GetNameByYear(int year)
    {
        return Name;
    }
}