internal class ProgramSettings
{
    public ProgramSettings()
    {
        Height = 100;
        Width = 100;
        Scale = 1;
        SystemRadius = 3;
    }
    public int Height { get; set; }
    public int Width { get; set; } 

    public string? Center { get; set; } // system name, x,y

    public int Scale { get; set; }

    public string? SystemTitles { get; set; } // none, name
    public string? SystemSubtitles { get; set; } // none, faction
    public int SystemRadius { get; set; }

    public string? SystemColors { get; set; } // faction, #xxxxxx

    public string? LinkColors { get; set; } // faction, #xxxxxx
}