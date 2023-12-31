# Welcome Stalker

Stalker Modding Helper is a little tool which allows you to copy the mods you are working on to your dev S.T.A.L.K.E.R. Anomaly installation and either launches the game when it's not running + loads a save automatically or just sends a command to the game to reload your testing save. The tool only copies files which were changed by using MD5 hash validation. 

This tool does not fully work with MO2 as it can't force it to launch the game.

Here's an example:
- My Anomaly is installed at **`D:\Anomaly Development`**
- My mod is located at **`D:\Zone Link\ZoneLinkMod`**

![Directories](https://i.imgur.com/Nd7NYDs.png)

I am able to work in my mod's directory and when I'm ready to test the changes I can run the Stalker Modding Helper and copy the files to my Anomaly installation and automatically start the game with an automatic save load or just reload the save:

**Click on the picture below to watch the showcase**

[![Stalker Modding Helper Showcase](https://img.youtube.com/vi/8EUCIZlSeWg/0.jpg)](https://www.youtube.com/watch?v=8EUCIZlSeWg)

## Requirements

In order for the application to run you need to download and install [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer)

## Installation

Simply unpack the software wherever you want to. You can either use a single instance or multiple instances per each mod.

Afterwards configure **`Config.ini`** and close Anomaly if it's already running as the software will also create a special script **`stalker_modding_helper.script`** in your Anomaly installation when it runs which is required for save reloading.

## Configuration

Initially the tool comes with a very simple configuration in the form of a **`Config.ini`** file which has to be placed next to **`StalkerModdingHelper.exe`**

### Currently supported values:
- **`StalkerPath`** - Defines the Anomaly installation directory (where the bin and gamedata folders reside). 
- **`StalkerExecutable`** - Defines either the path of the executable you want to run or just its name.
- **`ModPath`** - Comma separated mod directories (where the bin and gamedata folders reside) which you want to copy to your Anomaly installation.
- **`SkipExtension`** - Comma separated file extensions which you don't want to copy automatically.
- **`SaveName`** - The name of the save file you want to load. It's good to name it something simple like `testing`.
- **`AutoRun`** - If set to True will automatically start Anomaly or reload the save.

### Example Configuration
```
StalkerPath=D:\Anomaly Development
StalkerExecutable=AnomalyDX11
ModPath=D:\Zone Link\ZoneLinkMod
SkipExtension=dds,seq,pe,pg,ogf
SaveName=testing
AutoRun=True
```

## Quality of Life

If you are using simple text editors like Notepad++ or SublimeText you have to run the tool manually unless your software supports some sort of custom macros or you have other tools like docks or hardware like Elgato Stream Deck. You can also try getting some custom keyboard macro editor to start the application with a specific key.

If on the other hand you are using a proper IDE and you have a proper solution with your mod then you can create a new configuration the same way Debug and Release configurations work. Simply point to the StalkerModdingHelper.exe. 

This will make it so that it will work the same way it does on the video above where running your project will run Anomaly or reload the save.
