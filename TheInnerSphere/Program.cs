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

        Console.Write("Reading data files...");

        PlanetInfoRepository? planetRepo = null;
        if (String.Equals(map, "all"))
        {
            planetRepo = new PlanetInfoRepository("../data/systems.tsv");
        }
        else
        {
            planetRepo = new PlanetInfoRepository($"../extracted/{map}.data", true);
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

        // mid-3152 map rectangles
        /*plotter.Add(new Rectangle(new SystemCoordinates(-353.152,490.786), new SystemCoordinates(-20.947,144.710)));
        plotter.Add(new Rectangle(new SystemCoordinates(-421.047,-321.082), new SystemCoordinates(-12.415,10.294)));
        plotter.Add(new Rectangle(new SystemCoordinates(-198.534,-394.997), new SystemCoordinates(39.275,-247.142)));
        plotter.Add(new Rectangle(new SystemCoordinates(-48.602,11.846), new SystemCoordinates(185.100,498.766)));
        plotter.Add(new Rectangle(new SystemCoordinates(44.199,-72.946), new SystemCoordinates(509.843,116.275)));*/

        var outputFile = $"output.{map}.svg";
        plotter.Write(outputFile);

        Console.WriteLine($" Saved to {outputFile}!");
    }
}