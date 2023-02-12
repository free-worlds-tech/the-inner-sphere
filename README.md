# The Inner Sphere

This repo has a simple map plotter for BattleTech. It accepts several options to control map generation. The map opton is the name of a file in the `./system-data/` directory such as `3025`, `3151`, or `all`. The settings option controls the details of the generated svg file. The overlays option can be used to add elements, such as rectangles, to the generated map. In order to see all available options, use `--help`.

## Examples

```
dotnet run -- --map 3025
```

```
dotnet run -- --map 3049 --settings mini
```

```
dotnet run -- --map 3152 --settings mini --overlays mid-3152-maps
```

```
dotnet run -- --help
```

## Custom Settings

Program settings are contained in a json file that can specify various properties to use when creating a map. The settings file is defined in [`ProgramSettings.cs`](TheInnerSphere/ProgramSettings.cs). The repo contains several sample files:

- [`default.json`](TheInnerSphere/default.json): The default settings for the program. It creates a map that shows the entire Inner Sphere and near Periphery using faction colors. Systems are labeled with their name and faction.
- [`full.json`](TheInnerSphere/full.json): These settings match the default other than expanding the dimensions of the map to include all known systems.
- [`mini.json`](TheInnerSphere/mini.json): The mini settings create a much more compact view of the Inner Sphere without any text. These maps are just meant to show the general shape of the various interstellar nations.
- [`prototype.json`](TheInnerSphere/prototype.json): These settings are a work-in-progress on emulating some elements of the style of the poster maps from Tamar Rising and Empire Alone. Compared to the default settings, it produces a more compact map with larger text.

# Extractor

This helper app extracts map data from the data files in `./data/` to markdown table files in `./systam-data/` and `./faction-data/`. Those markdown tables are then used by the map generator.
