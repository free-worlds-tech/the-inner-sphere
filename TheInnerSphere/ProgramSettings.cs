internal class ProgramSettings
{
    public ProgramSettings()
    {
        Height = 100;
        Width = 100;
        Scale = 1;
        SystemRadius = 3;
        TitleFontSize = 16;
        SubtitleFontSize = 6;
        LinkStrokeWidth = 1;
        LinkOpacity = 0.5;
    }
    public int Height { get; set; }
    public int Width { get; set; } 

    public string? Center { get; set; } // system name

    public int Scale { get; set; }

    public string? SystemTitles { get; set; } // none, name, static-name
    public int TitleFontSize { get; set; }
    public string? SystemSubtitles { get; set; } // none, faction, alt-names
    public int SubtitleFontSize { get; set; }
    public int SystemRadius { get; set; }

    public string? SystemColors { get; set; } // faction, #xxxxxx

    public string? LinkColors { get; set; } // faction, #xxxxxx
    public double LinkStrokeWidth { get; set; }
    public double LinkOpacity { get; set; }

    public bool IncludeAbandonedSystems { get; set; }
    public bool IncludeUndiscoveredSystems { get; set; }
    public bool IncludeSystemsWithUnknownStatus { get; set; }
}