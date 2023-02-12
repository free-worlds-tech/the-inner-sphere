# The Inner Sphere

This repo has a simple map plotter for BattleTech. It is written in C# using .Net 6. The console app requires a single parameter that is which map to generate. The map names are the files in the `./system-data/` directory such as `3025`, `3151`, or `all`. If you want to use alternate settings for map generation, a second parameter can be passed with the path to a settings json file. A third argument can provide an overlays json file to add elements, such as rectangles, to the generated map.

## Examples

```
dotnet run 3025
```

```
dotnet run 3049 mini
```

```
dotnet run 3152 mini mid-3152-maps
```

## Custom Settings

Program settings are contained in a json file that can specify various properties to use when creating a map. The settings file is defined in [`ProgramSettings.cs`](TheInnerSphere/ProgramSettings.cs). The repo contains several sample files:

- [`default.json`](TheInnerSphere/default.json): The default settings for the program. It creates a map that shows the entire Inner Sphere and near Periphery using faction colors. Systems are labeled with their name and faction.
- [`full.json`](TheInnerSphere/full.json): These settings match the default other than expanding the dimensions of the map to include all known systems.
- [`mini.json`](TheInnerSphere/mini.json): The mini settings create a much more compact view of the Inner Sphere without any text. These maps are just meant to show the general shape of the various interstellar nations.
- [`prototype.json`](TheInnerSphere/prototype.json): These settings are a work-in-progress on emulating some elements of the style of the poster maps from Tamar Rising and Empire Alone. Compared to the default settings, it produces a more compact map with larger text.

# Extractor

This helper app extracts map data from the data files in `./data/` to markdown table files in `./systam-data/` and `./faction-data/`. Those markdown tables are then used by the map generator.
