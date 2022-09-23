internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("The Inner Sphere!");

        var planetRepo = new PlanetInfoRepository(args[0]);
        var factionRepo = new FactionInfoRepository(args[1]);

        var plotter = new SvgPlotter(4200, 4200);

        foreach (var id in planetRepo.GetPlanetIds())
        {
            var planet = planetRepo.GetPlanetInfo(id);
            var faction = factionRepo.GetFactionInfo(planet.Owners.GetOwner("2750"));

            if (faction.Id != "A" && faction.Id != "U" && faction.Id != "")
            {
                plotter.Add(planet.Coordinates.X, planet.Coordinates.Y, faction.Color);
            }

        }

        plotter.Write("output.svg");
    }
}