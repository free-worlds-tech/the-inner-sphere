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

        Extract(planetRepo, 3025, "3025");
        Extract(planetRepo, 3151, "3151");
        Extract(planetRepo, 3152, "3152");
        Extract(planetRepo, 3152, "3152x", new string[] {"3152", "3151"});
    }

    private static void Extract(PlanetInfoRepository planetRepo, int year, string mapName)
    {
        Extract(planetRepo, year, mapName, new string[] {mapName});
    }

    private static void Extract(PlanetInfoRepository planetRepo, int year, string filename, string[] mapNames)
    {
        var ids = planetRepo.GetPlanetIds();

        using (var writer = new StreamWriter($"{filename}.data"))
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