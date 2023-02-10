using System.Text.Json;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("The Inner Sphere - Data Extractor");

        Console.Write("Reading data files...");

        var planetRepo = new PlanetInfoRepository("../data/systems.tsv");

        Console.WriteLine(" Done!");

        Extract(planetRepo, 2271);
        Extract(planetRepo, 2317);
        Extract(planetRepo, 2319);
        Extract(planetRepo, 2341);
        Extract(planetRepo, 2367);
        Extract(planetRepo, 2571);
        Extract(planetRepo, 2596);
        Extract(planetRepo, 2750);
        Extract(planetRepo, 2765);
        Extract(planetRepo, 2767);
        Extract(planetRepo, 2783);
        Extract(planetRepo, 2786);
        Extract(planetRepo, 2821);
        Extract(planetRepo, 2822);
        Extract(planetRepo, 2830);
        Extract(planetRepo, 2864);
        Extract(planetRepo, 3025);
        Extract(planetRepo, 3030);
        Extract(planetRepo, 3040);
        Extract(planetRepo, 3049);
        Extract(planetRepo, 3050, "3050a");
        Extract(planetRepo, 3050, "3050b");
        Extract(planetRepo, 3050, "3050c");
        Extract(planetRepo, 3051);
        Extract(planetRepo, 3052);
        Extract(planetRepo, 3057);
        Extract(planetRepo, 3058);
        Extract(planetRepo, 3059, "3059a");
        Extract(planetRepo, 3059, "3059b");
        Extract(planetRepo, 3059, "3059c");
        Extract(planetRepo, 3059, "3059d");
        Extract(planetRepo, 3063);
        Extract(planetRepo, 3067);
        Extract(planetRepo, 3068);
        Extract(planetRepo, 3075);
        Extract(planetRepo, 3079);
        Extract(planetRepo, 3081);
        Extract(planetRepo, 3085);
        Extract(planetRepo, 3095);
        Extract(planetRepo, 3130);
        Extract(planetRepo, 3135);
        Extract(planetRepo, 3145);
        Extract(planetRepo, 3151);
        Extract(planetRepo, 3152);

        // Extract(planetRepo, 3152, "3152x", new string[] {"3152", "3151"});
    }

    private static void Extract(PlanetInfoRepository planetRepo, int year)
    {
        Extract(planetRepo, year, $"{year}", new string[] {$"{year}"});
    }

    private static void Extract(PlanetInfoRepository planetRepo, int year, string mapName)
    {
        Extract(planetRepo, year, mapName, new string[] {mapName});
    }

    private static void Extract(PlanetInfoRepository planetRepo, int year, string filename, string[] mapNames)
    {
        var ids = planetRepo.GetPlanetIds();

        using (var writer = new StreamWriter($"../extracted/{filename}.data"))
        {
            foreach(var id in ids)
            {
                var info = planetRepo.GetPlanetInfo(id);
                var name = info.GetNameByYear(year);
                var x = info.Coordinates.X;
                var y = info.Coordinates.Y;
                var owner = "?";
                var ownerNote = "";
                for (int i = 0; i < mapNames.Length; i++)
                {
                    var possibleOwner = info.Owners.GetOwner(mapNames[i]);
                    if (!String.IsNullOrEmpty(possibleOwner))
                    {
                        owner = possibleOwner;
                        ownerNote = info.Owners.GetOwnershipNote(mapNames[i]);
                        break;
                    }

                }

                writer.WriteLine($"{id}|{name}|{x}|{y}|{owner}|{ownerNote}");
            }
        }
    }
}