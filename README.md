
# FactorioModsManager

This program allows you to automatically download all factorio mods
for specific versions you can configure. It also has a few filtering
options to configure how many releasaes it should download and maintain
per mod and for how long, and if it should automatically delete no longer
maintained releases.

The project may be extended upon later down the road, however that is all it
does for now. The most useful and important feature would be to give the
program a `mod-list.json` file or a save file and it automatically puts all
the mods required into a folder, and downloads them if they are not maintaned.

Other than that, there are many things to do with this, it is basically a
copy of the mod portal, but local. Querying the summary for every single mod
could be done within a couple of milliseconds instead of hours having to request
each one indivitually from the portal api.

## General Behavior notes

The program keeps all information about all mods in a single `data.xml` file.
It has to read and write the entire file every single time when launching or
saving changes. Additionally, since it is literally just an xml file, this is
very wasteful storage space wise. However it is how it is right now.

# Features

## CMD Args

`--config PATH` tells the program to use a specific config file. `PATH` is a path
to a config file, including the file name and extension.
If this argument is not given, the program will read the file
`FactorioModsManagerConfigFilePath.txt` relative to the executable to get the config
path.

`--create-config` tells the program to only create a default config file, if it
doesn't already exist, and not do anything else. It is recommended to do this for
the first time running the program, see `Initial setup` at the bottom.

## Execution process

During execution a couple of things happen:
* Get the path as described for `--config PATH`
* If the program does not find said config file, it will create a default one
* If `--create-config` is set, the program aborts here
* The program reads `data.xml` in `<DataPath>` if it exists
* The program gets all mod entries from the portal
* If it finds a new or updated mod, it will add it to it's `data.xml` file
  (it only saves the `data.xml` file about every 5 minutes)
* The program determins which releases should be maintained for every mod
  (it enumerates the releases sorted by release date descending)
  * First checks against `<MinMaintainedReleases>`
  * Then checks against `<MaxMaintainedReleases>`
  * And finally against `<MaintainedDays>`
* The releases marked as maintained should exist in `<ModsPath>`
  * If they do not, the program will download the release from the portal
* If `<DeleteNoLongerMaintainedReleases>` is true,
  * The releases marked as not maintained should not exist in `<ModsPath>`.
    If they do, the program deletes them

# Initial setup

## Windows

* Download the binaries to wherever you like, such as
  `C:\Program Files\FactorioModsManager\` for instance to follow convention.
* Adjust the default config file path in `FactorioModsManagerConfigFilePath.txt`
  (Or leave it as default)
  Note that the program does not automatically create the directory for the config
  file when creating the file, to avoid it creating it somewhere you didn't want it
  to be
* Launch the program with the argument `--create-config`
* The default config file has now been created and you may configure it however you like
  (There is currently no documentation outside of this file for it's syntax and behavior)
* Make sure to supply `<FactorioUserName>` and `<FactorioUserToken>`. The program currently
  throws a generic `NotImplementedException` if they are invalid
* For future executions, don't provide the argument `--create-config` if you actually
  want it to do something

## Other platforms

I have no idea. Read this readme and use some kind of C# 8 .NET Core 3.0 compiler to
compile the source code. Once i learn some stuff about linux i may add more instructions.

# Cloning

When cloning the repository, make sure to run `git init submodules` and `git update submodules`
