internal class SimplePlanetFactionInfo : IPlanetFactionInfo
{
    public SimplePlanetFactionInfo(string owner, string ownershipNote)
    {
        _owner = owner;
        _ownershipNote = ownershipNote;
    }

    public string GetOwner()
    {
        return _owner;
    }

    public string GetOwnershipNote()
    {
        return _ownershipNote;
    }

    private string _owner;
    private string _ownershipNote;
}