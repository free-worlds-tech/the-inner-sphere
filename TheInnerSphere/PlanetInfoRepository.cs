using System.Linq;

internal class PlanetInfoRepository
{
    public PlanetInfoRepository(string file)
    {
        _planets = new Dictionary<uint, PlanetInfo>();
        ParseSimpleDataFile(file);

        var ids = _planets.Keys.ToArray();
        Array.Sort(ids);
        _sortedIds = ids;
    }

    public IEnumerable<uint> GetPlanetIds()
    {
        return _sortedIds;
    }

    public PlanetInfo GetPlanetInfo(uint id)
    {
        if (_planets.ContainsKey(id))
        {
            return _planets[id];
        }
        else
        {
            throw new ArgumentOutOfRangeException($"Planet {id} not found!");
        }
    }

    public IReadOnlyList<PlanetInfo> GetPlanetInfo(string name)
    {
        var matches = _planets.Values.Where((v) => String.Equals(name, v.Name, StringComparison.InvariantCultureIgnoreCase));
        return new List<PlanetInfo>(matches);
    }

    private void ParseSimpleDataFile(string file)
    {
        using (var reader = new StreamReader(file))
        {
            // Ignore two header lines
            reader.ReadLine();
            reader.ReadLine();

            var line = reader.ReadLine();
            while (line != null)
            {
                var entries = line.Split('|', StringSplitOptions.TrimEntries);

                _planets.Add(
                    UInt32.Parse(entries[0]), 
                    new PlanetInfo(
                        UInt32.Parse(entries[0]),
                        entries[1],
                        Double.Parse(entries[2]),
                        Double.Parse(entries[3]),
                        entries[4],
                        entries[5]
                    )
                );

                line = reader.ReadLine();
            }
        }
    }

    private Dictionary<uint, PlanetInfo> _planets;
    private IEnumerable<uint> _sortedIds;
    
}