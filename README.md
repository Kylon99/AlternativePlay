#  Alternative Play

## Introduction
This mod supports the following play styles
* ![IMG](AlternativePlay/Resources/BeatSaberColored64.png) Beat Saber
* ![IMG](AlternativePlay/Resources/DarthMaulColored64.png) Darth Maul
* ![IMG](AlternativePlay/Resources/BeatSpearColored64.png) Beat Spear

As well as the following Game Modifiers
* ![IMG](AlternativePlay/Resources/NoArrowsRandomColored64.png) No Arrows Random
* ![IMG](AlternativePlay/Resources/OneColorColored64.png) One Color
* ![IMG](AlternativePlay/Resources/NoArrows64.png) No Arrows
* ![IMG](AlternativePlay/Resources/StabNotesColored64.png) Stab Notes Coming Soon!


## Requirements
This mod depends on the following mods.  Download them at [BeatMods](https://beatmods.com).

* https://github.com/nike4613/BeatSaber-IPA-Reloaded/
* https://github.com/Kylemc1413/Beat-Saber-Utils
* https://github.com/monkeymanboy/BeatSaberMarkupLanguage

## Installation

Drop the AlternativePlay.dll file into your Plugins folder under your BeatSaber folder.  Start the game once to generate the INI file and edit the options you want.

## Changelog
### 0.1.0
* Initial build. 

## Author
* Kylon99 - Main modder

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Local Build
In order to build this project, please add a `AlternativePlay.csproj.user` file in the project directory and specify where your game is located on your disk:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!-- Change this path if necessary. Make sure it ends with a backslash. -->
    <GameDirPath>C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\</GameDirPath>
  </PropertyGroup>
</Project>
```

