# The Inner Sphere

This repo has a simple map plotter for BattleTech. It is written in C# using .Net 6. The console app requires a single parameter that is which map to generate. The map names are the columns in the `data/systems.tsv` file such as `3025` or `3151`. The program also supports passing `all` which will output a version with all of the systems in the data file without any faction coloring. If you want to generate a map zoomed to only show the Inner Sphere, add `zoomed` as a second parameter.

## Examples

```
dotnet run 3025
```

```
dotnet run 2750 zoomed
```