internal class FactionInfoRepository
{
    public FactionInfoRepository(string file)
    {
        _factions = new Dictionary<string, FactionInfo>();
        ParseDataFile(file);
    }

    public IEnumerable<string> GetFactionIds()
    {
        return _factions.Keys;
    }

    public FactionInfo GetFactionInfo(string id)
    {
        if (String.IsNullOrEmpty(id))
        {
            return new FactionInfo("", "Undefined", "#000000");
        }
        else if (_factions.ContainsKey(id))
        {
            return _factions[id];
        }
        else
        {
            throw new ArgumentOutOfRangeException($"Faction {id} not found!");
        }
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

                _factions.Add(entries[0], new FactionInfo(entries));

                line = reader.ReadLine();
            }
        }
    }

    private Dictionary<string, FactionInfo> _factions;
    
}