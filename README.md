#  Alternative Play

## Introduction
This mod supports the following play styles

### ![IMG](AlternativePlay/Public/BeatSaberColor64.png) Beat Saber
There are options to reverse the directions of each saber allowing for Reverse Style play.

### ![IMG](AlternativePlay/Public/DarthMaulColor64.png) Darth Maul
Two controller mode allows you to play Darth Maul without a stick, or use two trackers 
mounted on a stick to play a proper staff style Darth Maul. You can also use one controller 
or one tracker to play one handed Darth Maul.  Reverse the Maul direction for those who 
wish to have the left saber facing left in one controller mode.

An option allows you to use both triggers to separate the Darth Maul sabers in two controller 
mode, or one trigger in one controller mode.  Increase or decrease the separation amount 
to widen or narrow the Darth Maul saber positions.

### ![IMG](AlternativePlay/Public/BeatSpearColor64.png) Beat Spear

Demonstration Video: https://youtu.be/1ZGCbvZor1c

Play with one controller or tracker mounted on a stick for spear-like play.  Or use the Two Controller 
mode to play with a virtual spear drawn between both controllers.  Use the trigger to switch the 
controller to be the 'front' hand. The spear can hit any notes of any color.

### ![IMG](AlternativePlay/Public/NunchakuColor64.png) Nunchaku

Swing a simulated two segment nunchaku! Use the trigger to hold the nunchaku in that hand or press
both triggers to hold both segments in each hand.

### ![IMG](AlternativePlay/Public/BeatFlailColor64.png) Beat Flail

Swing the saber around on a chain like a flail! Longer chains are harder to swing and control
while shorter chains allow for less of a full arm swing.

### ![IMG](AlternativePlay/Public/NoArrowsColor64.png) No Arrows
### ![IMG](AlternativePlay/Public/OneColorColor64.png) One Color
### ![IMG](AlternativePlay/Public/NoSlidersColor64.png) No Sliders
### ![IMG](AlternativePlay/Public/NoArrowsRandomColor64.png) No Arrows Random
This functionality was broken by Beat Saber 1.12.  It may be restored one day.
### ![IMG](AlternativePlay/Public/TouchNotesColor64.png) Touch Notes


## Tracker Selection

You can now choose which trackers to use for Beat Saber, Darth Maul and Beat Spear without the need for OpenVR Input Emulator.  You can also adjust the position and rotation of the saber for when using these trackers.

## Contributors

* https://github.com/Kylon99
* https://github.com/Snow1226
* Special thanks to Nullpon for testing and fixes! https://github.com/nullpon16tera

## Requirements
This mod depends on the following mods.  Download them at [BeatMods](https://beatmods.com).

* https://github.com/nike4613/BeatSaber-IPA-Reloaded/
* https://github.com/Kylemc1413/Beat-Saber-Utils
* https://github.com/monkeymanboy/BeatSaberMarkupLanguage

## Installation

Drop the AlternativePlay.dll file into your Plugins folder under your BeatSaber folder.

## Changelog
### 0.7.4.1
- Added Updated UI to support multiple play mode settings

### 0.7.4
- Updated to support Beat Saber 1.21
- Added NoSliders game modifier option

### 0.7.3
- Fixed Game Modifiers to work with Noodle Extensions Maps
- Fixed early spawning notes not modified
- Released AlternativePlay icons under MIT license

### 0.7.2
- Fixed NoArrows and OneColor transform for Beat Saber 1.17.1

### 0.7.1
- Added ability to play Beat Flail with only one hand or with a sword

### 0.7.0
- Added new Nunchaku and Beat Flail play modes

### 0.6.3
- Updated code to restore compatibility with Custom Saber when playing One Color
- Fixed right saber not showing when use left saber set in Beat Saber
- Moved Remove Other Saber option to Beat Saber

### 0.6.2
- Checked for compatibility with Beat Saber 1.14, 1.15 and 1.16.1
- Added support for play styles during Multiplayer (Snow1226)
- Changed license to GPLv3
- Fixed DarthMaulHapticPatch NullReferenceException log spam
- Added support for TrueVR in VMCAvatar (Snow1226)

### 0.6.1
- Updated to support Beat Saber 1.14
- Changed code to avoid disabled sabers still hitting bombs in the base game
- Fixed Beat Saber mode to not move sabers unless reverse is used
- Enabled AlternativePlay functionality in multiplayer modes besides QuickPlay
- Cleaned up UI and removed unused icons

### 0.6.0
- Updated to support Beat Saber 1.13.4
- Fixed Touch Notes to patch the new NoteBasicCutInfoHelper

### 0.5.3
- Added room and noodle extension adjustment to saber tracking
- Fixed tracker select display to follow room rotation

### 0.5.2
- Updated to support Beat Saber 1.13.2
- Fixed saber positioning code after the changes in 1.13.2

### 0.5.1
- Restore the tracker feature in 0.4.2
- Changed the mode switching UI to be similar to 0.4.2.

### 0.5.0
- Game version 1.12.2 is supported
- Disabling Alternative Play in official multimode

### 0.4.2
- Added ability to rotate and change positions of the saber when using trackers

### 0.4.2
- Added ability to rotate and change positions of the saber when using trackers

### 0.4.1
- Added behavior to show tracker positions when selecting

### 0.4.0
- Added new options to use trackers for saber positions without the need for OpenVR Input Emulator
- Added option to enable or disable hand switching in Beat Spear
- Added button coloring on main view controller to indicate play mode
- Changed configuration to use JSON serialization

### 0.3.2
- Fixed manifest to reference correct Darth Maul icon
- Updated manifest to support Beat Saber 1.11.0

### 0.3.1
- Updated logging to use the new BSIPA4 logger
- Bumped version requirement of BS_Utils to 1.4.6

### 0.3.0
- Updated manifest to support Beat Saber 1.8.0
- Updated for BSIPA4 and Harmony2
- Rewrote controller input to support the new Unity XR System
- Changed two controller Darth Maul to separate with only one trigger

### 0.2.4
- Rotated other saber in two controller Darth Maul to face opposite direction
- Reversed haptic when using two controller Darth Maul and Reverse Maul Direction

### 0.2.3
- Fixed issue when splitting Darth Maul not restoring haptic feed back on off hand

### 0.2.2
- Fixed issue with causing Versus or Downloader mod settings buttons to stay disabled
- Fixed issue with clicking through menus too quickly causing UI to become unresponsive

### 0.2.1
- Renamed Stab Notes to Touch Notes
- Updated manifest to support Beat Saber 1.7.0

### 0.2.0
- Beat Saber: Split the options reverse sabers into one for each hand
- Stab Notes: Now working, thanks to Kylemc1413's Simple Hit code
- Darth Maul: Haptic feedback on the proper hand in One Controller mode now working again
- Bear Spear: Will now remove the other hand's saber when using default sabers

### 0.1.0
* Initial build. 


## License
This project is licensed under the GPLv3 License as of version 0.6.2 - see the [LICENSE](LICENSE) file for details.

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

