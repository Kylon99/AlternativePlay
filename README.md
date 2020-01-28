#  Alternative Play

## Introduction
This mod supports the following play styles

### ![IMG](AlternativePlay/Resources/BeatSaberColored64.png) Beat Saber
There are options to reverse the directions of each saber allowing for Reverse Style play.

### ![IMG](AlternativePlay/Resources/DarthMaulColored64.png) Darth Maul
Two controller mode allows you to play Darth Maul without a stick, or use two trackers mounted on a stick to play a proper staff style Darth Maul. You can also use one controller or one tracker to play one handed Darth Maul.  Reverse the Maul direction for those who wish to have the left saber facing left in one controller mode.

An option allows you to use both triggers to separate the Darth Maul sabers in two controller mode, or one trigger in one controller mode.  Increase or decrease the separation amount to widen or narrow the Darth Maul saber positions.

### ![IMG](AlternativePlay/Resources/BeatSpearColored64.png) Beat Spear

Demonstration Video: https://youtu.be/1ZGCbvZor1c

Play with one controller or tracker mounted on a stick for spear-like play.  Or use the Two Controller mode to play with a virtual spear drawn between both controllers.  Use the trigger to switch the controller to be the 'front' hand.

### ![IMG](AlternativePlay/Resources/NoArrowsRandomColored64.png) No Arrows Random
### ![IMG](AlternativePlay/Resources/OneColorColored64.png) One Color
### ![IMG](AlternativePlay/Resources/NoArrows64.png) No Arrows
### ![IMG](AlternativePlay/Resources/StabNotesColored64.png) Stab Notes Coming Soon!


## Requirements
This mod depends on the following mods.  Download them at [BeatMods](https://beatmods.com).

* https://github.com/nike4613/BeatSaber-IPA-Reloaded/
* https://github.com/Kylemc1413/Beat-Saber-Utils
* https://github.com/monkeymanboy/BeatSaberMarkupLanguage

## Installation

Drop the AlternativePlay.dll file into your Plugins folder under your BeatSaber folder.

## Changelog
### 0.2.0
- Beat Saber: Split the options reverse sabers into one for each hand
- Stab Notes: Now working, thanks to Kylemc1413's Simple Hit code
- Darth Maul: Haptic feedback on the proper hand in One Controller mode now working again
- Bear Spear: Will now remove the other hand's saber when using default sabers

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

