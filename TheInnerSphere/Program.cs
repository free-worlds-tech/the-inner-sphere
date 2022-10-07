using System.Text.Json;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("The Inner Sphere!");

        string map = "all";
        string settingsFile = "default.json";
        if (args.Length == 1)
        {
            map = args[0].ToLower();
        }
        else if (args.Length == 2)
        {
            map = args[0].ToLower();
            settingsFile = args[1];
        }
        else if (args.Length > 2)
        {
            Console.WriteLine("Unexpected number of arguments");
            return;
        }

        ProgramSettings settings = new ProgramSettings();
        using (var stream = new FileStream(settingsFile, FileMode.Open, FileAccess.Read))
        {
            ProgramSettings? deserialized = JsonSerializer.Deserialize<ProgramSettings>(stream);
            if (deserialized != null)
            {
                settings = deserialized;
            }
        }

        switch (map)
        {
            case "all":
            case "2271":
            case "2317":
            case "2319":
            case "2341":
            case "2367":
            case "2571":
            case "2596":
            case "2750":
            case "2765":
            case "2767":
            case "2783":
            case "2786":
            case "2821":
            case "2822":
            case "2830":
            case "2864":
            case "3025":
            case "3030":
            case "3040":
            case "3049":
            case "3050a":
            case "3050b":
            case "3050c":
            case "3051":
            case "3052":
            case "3057":
            case "3058":
            case "3059a":
            case "3059b":
            case "3059c":
            case "3059d":
            case "3063":
            case "3067":
            case "3068":
            case "3075":
            case "3079":
            case "3081":
            case "3085":
            case "3095":
            case "3130":
            case "3135":
            case "3145":
            case "3151":
            case "3152":
                break;
            default:
                Console.WriteLine("Unrecognized map name");
                return;
        }

        Console.Write("Reading data files...");

        var planetRepo = new PlanetInfoRepository("../data/systems.tsv");
        var factionRepo = new FactionInfoRepository("../data/factions.tsv");

        Console.WriteLine(" Done!");

        var centerCoordinates = new SystemCoordinates(0,0);
        if (settings.Center != null)
        {
            var matchingSystems = planetRepo.GetPlanetInfo(settings.Center);
            if (matchingSystems.Count == 1)
            {
                centerCoordinates = matchingSystems[0].Coordinates;
            }
            else if (matchingSystems.Count == 0)
            {
                Console.Error.WriteLine($"No system found with name: {settings.Center}");
                return;
            }
            else if (matchingSystems.Count > 1)
            {
                Console.Error.WriteLine($"Multiple systems found with name: {settings.Center}");
                return;
            }
        }


        
        SystemColorMapping? systemPalette = null;
        if (map != "all" && String.Equals(settings.SystemColors, "faction", StringComparison.InvariantCultureIgnoreCase))
        {
            systemPalette = (PlanetInfo system) => {
                var faction = factionRepo.GetFactionInfo(system.Owners.GetOwner(map));
                return faction.Color;
            };
        }
        else
        {
            systemPalette = (PlanetInfo system) => {
                if (!String.IsNullOrEmpty(settings.SystemColors))
                {
                    return settings.SystemColors;
                }
                else
                {
                    return "#ffffff";
                }
            };
        }

        LinkColorMapping? linkPalette = null;
        if (map != "all" && String.Equals(settings.LinkColors, "faction", StringComparison.InvariantCultureIgnoreCase))
        {
            linkPalette = (PlanetInfo system1, PlanetInfo system2) => {
                var owner1 = system1.Owners.GetOwner(map);
                var owner2 = system2.Owners.GetOwner(map);

                if (String.Equals(owner1, owner2) && !String.Equals("I", owner1) && !String.Equals("A", owner1))
                {
                    return factionRepo.GetFactionInfo(owner1).Color;
                }
                else
                {
                    return factionRepo.GetFactionInfo("D").Color;
                }
            };
        }
        else
        {
            linkPalette = (PlanetInfo system1, PlanetInfo system2) => {
                if (!String.IsNullOrEmpty(settings.LinkColors))
                {
                    return settings.LinkColors;
                }
                else
                {
                    return "#ffffff";
                }
            };
        }

        SystemTitleMapping? titleMapping = null;
        if (String.Equals(settings.SystemTitles, "name", StringComparison.InvariantCultureIgnoreCase))
        {
            titleMapping = (PlanetInfo system) => {
                return system.Name;
            };
        }
        else if (String.Equals(settings.SystemTitles, "none", StringComparison.InvariantCultureIgnoreCase))
        {
            titleMapping = (PlanetInfo system) => {
                return "";
            };
        }
        else
        {
            Console.Error.WriteLine($"Unsupported title type: {settings.SystemSubtitles}");
            return;
        }

        SystemSubtitleMapping? subtitleMapping = null;
        if (map != "all" && String.Equals(settings.SystemSubtitles, "faction", StringComparison.InvariantCultureIgnoreCase))
        {
            subtitleMapping = (PlanetInfo system) => {
                var faction = factionRepo.GetFactionInfo(system.Owners.GetOwner(map));
                return faction.Name.ToUpper();
            };
        }
        else if (map == "all" || String.Equals(settings.SystemSubtitles, "none", StringComparison.InvariantCultureIgnoreCase))
        {
            subtitleMapping = null;
        }
        else
        {
            Console.Error.WriteLine($"Unsupported subtitle type: {settings.SystemSubtitles}");
            return;
        }

        ImportantWorldMapping importantWorldMapping = (PlanetInfo system) => {
            var note = system.Owners.GetOwnershipNote(map);
            if (note.ToLower().Contains("faction capital"))
            {
                return true;
            }
            else
            {
                return false;
            }
        };
        var plotterSettings = new PlotterSettings()
        {
            // From settings.json
            Width = settings.Width,
            Height = settings.Height,
            Center = centerCoordinates,
            Scale = settings.Scale,
            SystemPalette = systemPalette,
            SystemTitleMapping = titleMapping,
            SystemSubtitleMapping = subtitleMapping,
            SystemRadius = settings.SystemRadius,
            LinkPalette = linkPalette,
            LinkStrokeWidth = settings.LinkStrokeWidth,
            PrimaryFontSize = settings.TitleFontSize,
            SecondaryFontSize = settings.SubtitleFontSize,

            // Todo...
            ImportantWorldMapping = importantWorldMapping,
        };

        Console.Write("Creating map...");
        var plotter = new SvgPlotter(plotterSettings);
        foreach (var id in planetRepo.GetPlanetIds())
        {
            var planet = planetRepo.GetPlanetInfo(id);

            var plotPlanet = false;

            if (map == "all")
            {
                plotPlanet = true;
            }
            else
            {
                var faction = factionRepo.GetFactionInfo(planet.Owners.GetOwner(map));
                
                plotPlanet = true;
                if (!settings.IncludeAbandonedSystems && faction.Id == "A")
                {
                    plotPlanet = false;
                }
                if (!settings.IncludeUndiscoveredSystems && faction.Id == "U")
                {
                    plotPlanet = false;
                }
                if (!settings.IncludeSystemsWithUnknownStatus && faction.Id == "")
                {
                    plotPlanet = false;
                }
            }

            if (plotPlanet)
            {
                plotter.Add(planet);
            }
        }

        plotter.Write("output.svg");

        Console.WriteLine(" Saved to output.svg!");
    }
}