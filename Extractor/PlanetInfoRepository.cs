internal class PlanetInfoRepository
{
    public PlanetInfoRepository(string file)
    {
        _planets = new Dictionary<uint, PlanetInfo>();
        ParseDataFile(file);

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

    private void ParseDataFile(string file)
    {
        using (var reader = new StreamReader(file))
        {
            // Ignore header line
            var line = reader.ReadLine();

            line = reader.ReadLine();
            while (line != null)
            {
                var entries = line.Split('\t', StringSplitOptions.TrimEntries);

                _planets.Add(UInt32.Parse(entries[0]), new PlanetInfo(entries));

                line = reader.ReadLine();
            }
        }
    }

    private Dictionary<uint, PlanetInfo> _planets;
    private IEnumerable<uint> _sortedIds;
    
}