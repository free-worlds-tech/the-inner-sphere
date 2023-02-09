internal class PlanetFactionInfo
{
    public PlanetFactionInfo(string[] entries)
    {
        // 0    1           2               3   4   5       6           7               8       9       10      11      12      13      14      15      16      17      18      19      20      21      22      23      24      25      26      27      28      29      30      31      32      33      34      35      36      37      38      39      40      41      42      43      44      45      46      47      48      49      50      51
        // iD	systemName	alternateName	x	y	size	sarnaLink	distance (LY)	2271	2317	2319	2341	2367	2571	2596	2750	2765	2767	2783	2786	2821	2822	2830	2864	3025	3030	3040	3049	3050a	3050b	3050c	3051	3052	3057	3058	3059a	3059b	3059c	3059d	3063	3067	3068	3075	3079	3081	3085	3095	3130	3135	3145	3151	3152

        _owners = new Dictionary<string, string>();

        _owners.Add("2271", entries[8]);
        _owners.Add("2317", entries[9]);
        _owners.Add("2319", entries[10]);
        _owners.Add("2341", entries[11]);
        _owners.Add("2367", entries[12]);
        _owners.Add("2571", entries[13]);
        _owners.Add("2596", entries[14]);
        _owners.Add("2750", entries[15]);
        _owners.Add("2765", entries[16]);
        _owners.Add("2767", entries[17]);
        _owners.Add("2783", entries[18]);
        _owners.Add("2786", entries[19]);
        _owners.Add("2821", entries[20]);
        _owners.Add("2822", entries[21]);
        _owners.Add("2830", entries[22]);
        _owners.Add("2864", entries[23]);
        _owners.Add("3025", entries[24]);
        _owners.Add("3030", entries[25]);
        _owners.Add("3040", entries[26]);
        _owners.Add("3049", entries[27]);
        _owners.Add("3050a", entries[28]);
        _owners.Add("3050b", entries[29]);
        _owners.Add("3050c", entries[30]);
        _owners.Add("3051", entries[31]);
        _owners.Add("3052", entries[32]);
        _owners.Add("3057", entries[33]);
        _owners.Add("3058", entries[34]);
        _owners.Add("3059a", entries[35]);
        _owners.Add("3059b", entries[36]);
        _owners.Add("3059c", entries[37]);
        _owners.Add("3059d", entries[38]);
        _owners.Add("3063", entries[39]);
        _owners.Add("3067", entries[40]);
        _owners.Add("3068", entries[41]);
        _owners.Add("3075", entries[42]);
        _owners.Add("3079", entries[43]);
        _owners.Add("3081", entries[44]);
        _owners.Add("3085", entries[45]);
        _owners.Add("3095", entries[46]);
        _owners.Add("3130", entries[47]);
        _owners.Add("3135", entries[48]);
        _owners.Add("3145", entries[49]);
        _owners.Add("3151", entries[50]);
        _owners.Add("3152", entries[51]);
    }

    public string GetOwner(string map)
    {
        if (_owners.ContainsKey(map))
        {
            var ownerString = _owners[map].Split(',', StringSplitOptions.TrimEntries)[0];
            var paren = ownerString.IndexOf('(');
            if (paren >= 0)
            {
                return ownerString.Substring(0, paren);
            }
            else
            {
                return ownerString;
            }
        }
        else
        {
            return "Undefined";
        }
    }

    public string GetOwnershipNote(string map)
    {
        if (_owners.ContainsKey(map))
        {
            var entries =  _owners[map].Split(',', StringSplitOptions.TrimEntries);
            if (entries.Length > 1)
            {
                return String.Join(", ", entries.Take(new Range(1,entries.Length)));
            }
            else
            {
                return "";
            }
        }
        else
        {
            return "";
        }
    }

    private Dictionary<string, string> _owners;
}