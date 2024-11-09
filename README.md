# Welcome Stalker

Stalker Modding Helper is a little tool which allows you to copy the mods you are working on to your dev S.T.A.L.K.E.R. Anomaly installation and either launches the game when it's not running + loads a save automatically or just sends a command to the game to reload your testing save. The tool only copies files which were changed by using MD5 hash validation. 

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

Afterwards configure **`StalkerModdingHelper.ini`** and close Anomaly if it's already running as the software will also create a special script **`stalker_modding_helper.script`** in your Anomaly installation when it runs which is required for save reloading.



## Configuration

Initially the tool comes with a very simple configuration in the form of a **`StalkerModdingHelper.ini`** file which has to be placed next to **`StalkerModdingHelper.exe`**

With the release of the 2.0 version the software supports directly working on a Stalker installation as well as working through Mod Organizer 2.

The configuration for each of those methods is different therefore make sure that you will follow one of the guides below, not both.

The config file underwent changes in terms of its structure. Now it supports sections very much like in stalker's LTX files.

The base format is now a **`[StalkerModdingHelper]`** section which contains settings required for launching the game.

As well as an infinite amount of mod sections like **`[My Cool Mod]`** which contain mod specific settings allowing multiple mods to be supported.

The order in which the mods are listed in the config file dictates the order in which they will be either copied to Stalker or listed on the mod list.



## Currently supported [StalkerModdingHelper] section values:

- **`LaunchType`** - This setting changes how the game gets launched as well as what the other values from this section do.
  - **1** - The game gets launched directly via the specified Stalker executable.
  - **2** - The game gets launched through Mod Organizer 2 using the specified executable.

- **`LaunchPath`** - The base path to launch from. This path differs based on which `LaunchType` was selected.
  - `LaunchType` **1** - Root directory of Stalker installation.
  - `LaunchType` **2** - Root directory of Mod Organizer 2

- **`ExecutableName`** - The name of the executable to use. This name differs based on which `LaunchType` was selected.
  - `LaunchType` **1** - The name of the executable from the bin folder. For example `AnomalyDX11`.
  - `LaunchType` **2** - The name of the executable inside of Mod Organizer 2. For example `Anomaly (DX11)`.

- **`ProfileName`** - (Only required for `LaunchType` **2**). The name of the MO2 profile you want to use. Usually `Default`.

- **`SaveName`** - The name of the save file you want to load. It's good to name it something simple like `testing`.

- **`AutoRun`** - If set to 1 the software will automatically start the game or reload the save.



## Currently supported mod section values:

- **`ModPath`** - The root directory of your mod (the one containing the bin/db/gamedata folders).

- **`SkipExtension`** - Comma separated file extensions which you don't want to compare/copy. Useful for avoiding comparing/copying huge files which never change.



## Method 1 - Working with Stalker directly

The process is pretty much the same as before with the only difference being the config being slightly different.

In this case we have 2 mods, `My Cool Mod` and `My Amazing Mod` which will be copied over in the order they are listed in.

Example Configuration
```
[StalkerModdingHelper]
LaunchType=1
LaunchPath=D:\Anomaly
SaveName=testing
AutoRun=1
ExecutableName=AnomalyDX11

[My Cool Mod]
ModPath=D:\Mods\My Cool Mod

[My Amazing Mod]
ModPath=D:\Mods\My Amazing Mod
SkipExtension=dds,ogf
```

## Method 2 - Working with Mod Organizer 2
In the case of Mod Organizer 2 the process is slightly more complicated.

This only works if your mods and configs are stored inside of the Mod Organizer 2 directory like: 
- `D:\{MO2RootPath}\ModOrganizer.exe`
- `D:\{MO2RootPath}\ModOrganizer.ini`
- `D:\{MO2RootPath}\profiles\{SomeProfile}\modlist.txt`
- `D:\{MO2RootPath}\mods`

I might add the option to customize the paths later on.

First of all we need to create a new executable which will contain special arguments allowing us to load the save file automatically.

To do that we need to go to the `Modify Executables` window which can be accessed via the dropdown and `Edit` option.

Afterwards we want to select the executable we want to copy (For example `Anomaly (DX11)`), click on the plus button above, and select `Clone selected`.

![image](https://github.com/user-attachments/assets/3002419e-a033-4646-a43a-9dd31b2d32e7)

Now we can rename our newly created executable to let's say `Anomaly Dev`

![image](https://github.com/user-attachments/assets/02c328be-616f-4a9d-a25a-27a3cfc48b1c)

You can also copy the same arguments as on the screenshot but they will also be updated automatically by the software upon each run.

`-dbg -cls -nocache -start server(testing/single/alife/load) client(localhost)`

```diff
- Afterwards close Mod Organizer 2 as it will be overwriting changes to files if it stays open.
- Always keep Mod Organizer 2 closed when you are using this tool as it will conflict with file changes done by this tool.
```

**`One important note here is that the names of mod sections will be used to create mod folders as well as entries on the mod list. The order also matters.`**
**`If the mod folders in MO2 already exist and you want to copy into them then make sure that the name of the mod section in StalkerModdingHelper.ini matches them.`**

Example Configuration
```
[StalkerModdingHelper]
LaunchType=2
LaunchPath=D:\MO2
SaveName=testing
AutoRun=1
ExecutableName=Anomaly Dev
ProfileName=Default

[My Cool Mod]
ModPath=D:\Mods\My Cool Mod

[My Amazing Mod]
ModPath=D:\Mods\My Amazing Mod
SkipExtension=dds,ogf
```

## Quality of Life

If you are using simple text editors like Notepad++ or SublimeText you have to run the tool manually unless your software supports some sort of custom macros or you have other tools like docks or hardware like Elgato Stream Deck. You can also try getting some custom keyboard macro editor to start the application with a specific key.

If on the other hand you are using a proper IDE and you have a proper solution with your mod then you can create a new configuration the same way Debug and Release configurations work. Simply point to the StalkerModdingHelper.exe. 

This will make it so that it will work the same way it does on the video above where running your project will run Anomaly or reload the save.
