internal class FactionInfo
{
    public FactionInfo(string id, string name, string color)
    {
        Id = id;
        Name = name;
        Color = color;
    }
    
    public FactionInfo(string[] entries)
    {
        // 0    1           2
        // ID	factionName	color	foundingYear	dissolutionYear	2271	2317	2319	2341	2367	2571	2596	2750	2765	2767	2783	2786	2821	2822	2830	2864	3025	3030	3040	3049	3050a	3050b	3050c	3051	3052	3057	3058	3059a	3059b	3059c	3059d	3063	3067	3068	3075	3079	3081	3085	3095	3130	3135	3145	3151	3152

        Id = entries[0];
        Name = entries[1];
        Color = entries[2];
    }

    public string Name { get; }
    public string Id { get;  }
    public string Color { get; }
}