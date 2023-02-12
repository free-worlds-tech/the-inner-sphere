﻿using System.Text.Json;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("The Inner Sphere!");

        string map = "all";
        string settingsName = "default";
        string? overlaysName = null;
        if (args.Length == 1)
        {
            map = args[0].ToLower();
        }
        else if (args.Length == 2)
        {
            map = args[0].ToLower();
            settingsName = args[1];
        }
        else if (args.Length == 3)
        {
            map = args[0].ToLower();
            settingsName = args[1];
            overlaysName = args[2];
        }
        else if (args.Length > 3)
        {
            Console.WriteLine("Unexpected number of arguments");
            return;
        }

        string settingsFile = $"settings/{settingsName}.json";
        ProgramSettings settings = new ProgramSettings();
        using (var stream = new FileStream(settingsFile, FileMode.Open, FileAccess.Read))
        {
            ProgramSettings? deserialized = JsonSerializer.Deserialize<ProgramSettings>(stream);
            if (deserialized != null)
            {
                settings = deserialized;
            }
        }

        Overlays overlays = new Overlays();
        if (!String.IsNullOrEmpty(overlaysName))
        {
            string overlaysFile = $"../overlays/{overlaysName}.json";
            
            using (var stream = new FileStream(overlaysFile, FileMode.Open, FileAccess.Read))
            {
                Overlays? deserialized = JsonSerializer.Deserialize<Overlays>(stream);
                if (deserialized != null)
                {
                    overlays = deserialized;
                }
            }
        }

        Console.Write("Reading data files...");

        var planetRepo = new PlanetInfoRepository($"../extracted/{map}.table.md");
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
        if (String.Equals(settings.SystemColors, "faction", StringComparison.InvariantCultureIgnoreCase))
        {
            systemPalette = (PlanetInfo system) => {
                var owner = system.Owners.GetOwner();
                if (String.Equals("?", owner))
                {
                    return "#ffffff";
                }
                var faction = factionRepo.GetFactionInfo(owner);
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
        if (String.Equals(settings.LinkColors, "faction", StringComparison.InvariantCultureIgnoreCase))
        {
            linkPalette = (PlanetInfo system1, PlanetInfo system2) => {
                var owner1 = system1.Owners.GetOwner();
                var owner2 = system2.Owners.GetOwner();

                if (String.Equals(owner1, owner2) && !String.Equals("I", owner1) && !String.Equals("A", owner1) && !String.Equals("?", owner1))
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
        if (String.Equals(settings.SystemSubtitles, "faction", StringComparison.InvariantCultureIgnoreCase))
        {
            subtitleMapping = (PlanetInfo system) => {
                var owner = system.Owners.GetOwner();
                if (String.Equals(owner, "?"))
                {
                    return "UNKNOWN";
                }
                var faction = factionRepo.GetFactionInfo(owner);
                return faction.Name.ToUpper();
            };
        }
        else if (String.Equals(settings.SystemSubtitles, "none", StringComparison.InvariantCultureIgnoreCase))
        {
            subtitleMapping = null;
        }
        else
        {
            Console.Error.WriteLine($"Unsupported subtitle type: {settings.SystemSubtitles}");
            return;
        }

        ImportantWorldMapping importantWorldMapping = (PlanetInfo system) => {
            var note = system.Owners.GetOwnershipNote();
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
                var faction = factionRepo.GetFactionInfo(planet.Owners.GetOwner());
                
                plotPlanet = true;
                if (!settings.IncludeAbandonedSystems && faction.Id == "A")
                {
                    plotPlanet = false;
                }
                if (!settings.IncludeUndiscoveredSystems && faction.Id == "U")
                {
                    plotPlanet = false;
                }
                if (!settings.IncludeSystemsWithUnknownStatus && (faction.Id == "" || faction.Id == "?"))
                {
                    plotPlanet = false;
                }
            }

            if (plotPlanet)
            {
                plotter.Add(planet);
            }
        }

        foreach (var rect in overlays.Rectangles)
        {
            plotter.Add(rect);
        }

        if (!Directory.Exists("../output"))
        {
            Directory.CreateDirectory("../output");
        }
        var outputFile = $"../output/{map}.{settingsName}.svg";
        if (!String.IsNullOrEmpty(overlaysName))
        {
            outputFile = $"../output/{map}.{settingsName}.{overlaysName}.svg";
        }
        plotter.Write(outputFile);

        Console.WriteLine($" Saved to {outputFile}!");
    }
}