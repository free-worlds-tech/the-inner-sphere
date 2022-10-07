# The Inner Sphere

This repo has a simple map plotter for BattleTech. It is written in C# using .Net 6. The console app requires a single parameter that is which map to generate. The map names are the columns in the `data/systems.tsv` file such as `3025` or `3151`. The program also supports passing `all` which will output a version with all of the systems in the data file without any faction coloring. If you want to use alternate settings for map generation, a second parameter can be passed with the path to a settings json file.

## Examples

```
dotnet run 3025
```

```
dotnet run 3049 full.json
```