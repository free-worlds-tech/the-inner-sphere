internal class FactionInfoRepository
{
    public FactionInfoRepository(string file)
    {
        _factions = new Dictionary<string, FactionInfo>();
        ParseDataFile(file);
    }

    public FactionInfo GetFactionInfo(string id)
    {
        if (_factions.ContainsKey(id))
        {
            return _factions[id];
        }
        else
        {
            return new FactionInfo("", "Undefined", "#000000");
        }
    }

    private void ParseDataFile(string file)
    {
        using (var reader = new StreamReader(file))
        {
            // Ignore header lines
            reader.ReadLine();
            reader.ReadLine();

            var line = reader.ReadLine();
            while (!String.IsNullOrWhiteSpace(line))
            {
                var entries = line.Split('|', StringSplitOptions.TrimEntries);

                _factions.Add(entries[0], new FactionInfo(entries[0], entries[1], entries[2]));

                line = reader.ReadLine();
            }
        }
    }

    private Dictionary<string, FactionInfo> _factions;
    
}