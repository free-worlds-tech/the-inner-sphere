using System.Globalization;
using System.Text.RegularExpressions;

internal class PlanetInfo
{
    public PlanetInfo(string[] entries)
    {
        // 0    1           2               3   4   5       6           7               8       9       10      11      12      13      14      15      16      17      18      19      20      21      22      23      24      25      26      27      28      29      30      31      32      33      34      35      36      37      38      39      40      41      42      43      44      45      46      47      48      49      50      51
        // iD	systemName	alternateName	x	y	size	sarnaLink	distance (LY)	2271	2317	2319	2341	2367	2571	2596	2750	2765	2767	2783	2786	2821	2822	2830	2864	3025	3030	3040	3049	3050a	3050b	3050c	3051	3052	3057	3058	3059a	3059b	3059c	3059d	3063	3067	3068	3075	3079	3081	3085	3095	3130	3135	3145	3151	3152

        Id = UInt32.Parse(entries[0], CultureInfo.InvariantCulture);
        Name = entries[1];
        Coordinates = new SystemCoordinates(Double.Parse(entries[3], CultureInfo.InvariantCulture), Double.Parse(entries[4], CultureInfo.InvariantCulture));
        SarnaUrl = entries[6];
        Owners = new PlanetFactionInfo(entries);

        var altNames = new List<string>();
        if (!String.IsNullOrEmpty(entries[2]))
        {
            var alternates = entries[2].Split(",", StringSplitOptions.TrimEntries);
            foreach (var alternate in alternates)
            {
                altNames.Add(alternate);
            }
        }
        AlternateNames = altNames;
    }

    public string Name { get; }
    public IReadOnlyList<string> AlternateNames { get; }
    public uint Id { get;  }
    public SystemCoordinates Coordinates { get; }
    public string SarnaUrl { get; }
    public PlanetFactionInfo Owners { get; }

    public string GetNameByYear(int year)
    {
        var name = Name;
        foreach (var alternate in AlternateNames)
        {
            var pattern = @"([0-9]*)\+";
            if (Regex.IsMatch(alternate, pattern))
            {
                var match = Regex.Match(alternate, pattern);
                if (match.Groups.Count == 2 && !String.IsNullOrEmpty(match.Groups[1].Value))
                {
                    string yearString = match.Groups[1].Value;
                    int renameYear = Int32.Parse(yearString);
                    if (renameYear <= year)
                    {
                        return alternate.Substring(0, alternate.IndexOf('(')).Trim();
                    }
                }
            }
        }
        return name;
    }
}