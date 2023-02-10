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
            case "2271": // Free Worlds League founding year (partial)
            case "2317": // Federated Suns founding year (partial)
            case "2319": // Draconis Combine founding year (partial)
            case "2341": // Lyran Commonwealth founding year (partial)
            case "2367": // Capellan Confederation founding year (partial)
            case "2571": // Star League founding year
            case "2596": // End of Reunification War
            case "2750": // Beginning of the fall of the Star League
            case "2765": // Just before the Amaris Coup
            case "2767": // Amaris Empire
            case "2783": // Great House annexations of the Terran Hegemony
            case "2786": // Start of 1st Succession War
            case "2821": // Start of Operation Klondike
            case "2822": // End of 1st Succession War, End of Operation Klondike
            case "2830": // Start of 2nd Succession War
            case "2864": // End of 2nd Succession War
            case "3025": // End of 3rd Succession War
            case "3030": // End of 4th Succession War
            case "3040": // End of War of 3039
            case "3049": // Operation Revival: Periphery
            case "3050a": // Operation Revival: Wave 1
            case "3050b": // Operation Revival: Wave 2
            case "3050c": // Operation Revival: Wave 3
            case "3051": // Year of Peace
            case "3052": // End of Operation Revival
            case "3057": // Start of Operation Guerrero
            case "3058": // End of Operation Guerrero
            case "3059a": // Operation Bulldog: Wave 1
            case "3059b": // Operation Bulldog: Wave 2
            case "3059c": // Operation Bulldog: Wave 3
            case "3059d": // Operation Bulldog: Wave 4
            case "3063": // Start of Fed Com Civil War
            case "3067": // End of Fed Com Civil War
            case "3068": // Start of the Jihad
            case "3075": // Middle of the Jihad
            case "3079": // Waning years of the Jihad
            case "3081": // End of the Jihad
            case "3085": // End of the Wars of Reaving
            case "3095": // Early Republic
            case "3130": // Devlin Stone's Retirement
            case "3135": // Fortress Republic
            case "3145": // Return of Devlin Stone
            case "3151": // ilClan Trial
            case "3152": // Early ilClan Era (partial)
            case "3152x": // Early ilClan Era (fill-in-the-blanks)
                break;
            default:
                Console.WriteLine("Unrecognized map name");
                return;
        }

        Console.Write("Reading data files...");

        PlanetInfoRepository? planetRepo = null;
        if (String.Equals(map, "3152x"))
        {
            planetRepo = new PlanetInfoRepository("../Extractor/3152x.data", true);
        }
        else
        {
            planetRepo = new PlanetInfoRepository("../data/systems.tsv");
        }
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
            titleMapping = (PlanetInfo system) => 
            {
                if (map.Length >= 4 && Int32.TryParse(map.Substring(0,4), out int parsedYear))
                { 
                    return system.GetNameByYear(parsedYear);
                }
                else
                {
                    return system.Name;
                }
            };
        }
        else if (String.Equals(settings.SystemTitles, "static-name", StringComparison.InvariantCultureIgnoreCase))
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
        else if (String.Equals(settings.SystemSubtitles, "none", StringComparison.InvariantCultureIgnoreCase))
        {
            subtitleMapping = null;
        }
        else if (String.Equals(settings.SystemSubtitles, "alt-names", StringComparison.InvariantCultureIgnoreCase))
        {
            subtitleMapping = (PlanetInfo system) => {
                if (system.AlternateNames.Count > 0)
                {
                    return String.Join(", ", system.AlternateNames);
                }
                else
                {
                    return "";
                }
            };
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

        var outputFile = $"output.{map}.svg";
        plotter.Write(outputFile);

        Console.WriteLine($" Saved to {outputFile}!");
    }
}