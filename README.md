# LocationRunner plugin for PowerToys Run

This is a plugin for [PowerToys Run](https://learn.microsoft.com/windows/powertoys/run). It reads Key=Directory pairs `%localappdata%\LocationRunner\Locations.conf`, then collects all `.exe` files from those directories, those can then be searched with the plugin prefix `r:` followed by `<Key> <Executable>` and launched. Perhaps the screenshot below illustrates it better. ;)

![image](/images/Screenshot_2024-03-16_175735.png)

## Use Case

I'm working with a software that's coming with several frontend executables. I have to run these executables from different versions, which I'm storing in version specific directories, for bug reproduction or to figure out feature availability for a certain version.

For this, I'm also using

- taskbar toolbars, sadly, the feature is unavailable in Win 11
- powershell

Finding a way to make this available in PowerToys Run seemed like a nice idea. The default plugins don't cover this 100%, I think they make it a little difficult to select a specific version and I might have to add all directories to the PATH, which I don't want to do.

## Limitations, TODOs and potential ideas for the future

The plugin contains the bare minimum code to make it work. I don't know how much I'm going to work on this in the future. Feel free to just use this as reference if it's useful.

- the file format is key=directory, `#` in the beginning of a line is a comment
  - there is no plausibility check whatsoever
  - if there is no = the line is interpreted as directory with key "DEF"
- only `.exe` files will work. I could imagine using this as a shortcut for accessing version specific documentation
- I believe the action prefix `r:` I am using is free, but I have not spent too much effort with research ;)
- There is no localisation
- File system limitations apply, e.g. if you add a few thousand executables this way, it might be slow

## Usage

- Follow install instructions
- Populate `%localappdata%\LocationRunner\Locations.conf`
- Open PowerToys Run
- Type `r:`, optionally followed by search for filename
- Press enter to launch executable

## Install

Use one of the releases and copy the archive content to `%localappdata\PowerToys\RunPlugins\LocationRunner`.

Please note, if you are updating, and if you have just used PowerToys Run, you may have to exit PowerToys because otherwise the files may be in use.

Or you could follow the build instructions, build with Release configuration and copy the plugin from the release folder.

## Build

Before you do this, make sure you have sufficient disk space. I had to resize my VM disk twice. My PowerToys folder now contains almost 90 GiB with both Debug and Release config. :p

- Clone [PowerToys](https://github.com/microsoft/PowerToys)
- if you're worried about working with upstream changes, you could fork it and rebase as needed
- `cd <powertoys repository>\src\modules\launcher\Plugins`
- `git submodule add https://github.com/RobertMueller2/PowertoysRunLocationRunnerPlugin.git Community.PowerToys.Run.Plugin.LocationRunner`
- Add the newly added submodules .csproj file as project to the solution
- if you want to try it, you could build the solution with debug config and launch the PowerLauncher subproject, it should automatically load the LocationRunner plugin. Otherwise, build release config.

## Other remarks

This was helpful for me to understand how to create a module for PowerToys Run, as well as test and build it:

- [How to create a PowerToys Run plugin](https://senpai.club/how-to-create-a-powertoys-run-plugin/)
- [Winget plugin for PowerToys Run](https://github.com/bostrot/PowerToysRunPluginWinget)
- Shamelessly copied large sections of the README from my [BoilerplateText plugin](https://github.com/RobertMueller2/PowerToysRunPluginBoilerplate)


