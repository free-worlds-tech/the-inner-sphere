internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("The Inner Sphere!");

        string map = "all";
        int dimension = 4200;
        if (args.Length == 1)
        {
            map = args[0].ToLower();
        }
        else if (args.Length == 2 && String.Equals(args[1], "zoomed", StringComparison.OrdinalIgnoreCase))
        {
            map = args[0].ToLower();
            dimension = 1600;
        }
        else if (args.Length > 1)
        {
            Console.WriteLine("Unexpected number of arguments");
            return;
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


        Console.Write("Creating map...");
        SystemColorMapping? systemPalette = null;
        if (map != "all")
        {
            systemPalette = (PlanetInfo system) => {
                var faction = factionRepo.GetFactionInfo(system.Owners.GetOwner(map));
                return faction.Color;
            };
        }
        LinkColorMapping? linkPalette = null;
        if (map != "all")
        {
            linkPalette = (PlanetInfo system1, PlanetInfo system2) => {
                var owner1 = system1.Owners.GetOwner(map);
                var owner2 = system2.Owners.GetOwner(map);

                if (String.Equals(owner1, owner2) && !String.Equals("I", owner1))
                {
                    return factionRepo.GetFactionInfo(owner1).Color;
                }
                else
                {
                    return factionRepo.GetFactionInfo("D").Color;
                }
            };
        }
        SystemSubtitleMapping subtitleMapping = (PlanetInfo system) => {
            var faction = factionRepo.GetFactionInfo(system.Owners.GetOwner(map));
            return faction.Name.ToUpper();
        };
        var plotterSettings = new PlotterSettings()
        {
            Width = dimension,
            Height = dimension,
            SystemPalette = systemPalette,
            SystemSubtitleMapping = subtitleMapping,
            LinkPalette = linkPalette,
            //Scale = 15
            Scale = 1,
            SystemRadius = 3,
            IncludeSystemNames = false
        };
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

                if (faction.Id != "A" && faction.Id != "U" && faction.Id != "")
                {
                    plotPlanet = true;
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