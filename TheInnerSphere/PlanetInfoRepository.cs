internal class PlanetInfoRepository
{
    public PlanetInfoRepository(string file, bool simpleFile = false)
    {
        _planets = new Dictionary<uint, PlanetInfo>();
        if (simpleFile)
        {
            ParseSimpleDataFile(file);
        }
        else
        {
            ParseDataFile(file);
        }
    }

    public IEnumerable<uint> GetPlanetIds()
    {
        return _planets.Keys;
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

    private void ParseSimpleDataFile(string file)
    {
        using (var reader = new StreamReader(file))
        {
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
    
}