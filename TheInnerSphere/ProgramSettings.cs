internal class ProgramSettings
{
    public int Height { get; set; }
    public int Width { get; set; } 

    public string? Center { get; set; } // system name, x,y

    public string? SystemTitles { get; set; } // none, name
    public string? SystemSubtitles { get; set; } // none, faction

    public string? SystemColors { get; set; } // faction, #xxxxxx

    public string? LinkColors { get; set; } // faction, #xxxxxx
}