internal class FactionInfo
{
    public FactionInfo(string id, string name, string color)
    {
        Id = id;
        Name = name;
        Color = color;
    }

    public string Name { get; }
    public string Id { get;  }
    public string Color { get; }
}