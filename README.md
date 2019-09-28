**War Thunder Preset Randomizer** (or **WTPR**) is a work-in-progress Windows application providing a player with means to randomly select vehicles to play, with extra features down the road. In its current iteration (Alpha) baseline randomization is implemented based on game mode and vehicle branch preferences.

The source code contains a prototype console application that picks random vehicles within a selected game mode, nation, branch, and battle rating. There's also a utility that automates **WT Tools** (see *Requirements*) to perform a full cycle of unpacking and converting game files into JSON and CSV files - that's how [JSON File Changes](https://github.com/VitaliiAndreev/WarThunder_JsonFileChanges) and [JSON File Changes (Dev)](https://github.com/VitaliiAndreev/WarThunder_JsonFileChanges_DevClient) repositories are maintained.

### Requirements

In order to work **WTPR** requires an up-to-date version of the **War Thunder** client available at [Gaijin's website](https://warthunder.com/en) or [Steam](https://store.steampowered.com/app/236390/War_Thunder/), and a release of [Klensy](https://github.com/klensy/wt-tools/commits?author=klensy)'s **[WT Tools](https://github.com/klensy/wt-tools)**. Paths to both are inquired at the start of **WTPR**. Additionally, one might need to install a runtime version of [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472).

### Workflow

At the start **WTPR** scans the **War Thunder** client for its current version. With every new patch **WT Tools** are used to unpack data stored with the client and convert it into JSON and CSV, **WTPR** deserializes that data into entities and stores them in an SQLite database (a new database for every patch). From that point **WTPR** uses only the database unless it detects a new **War Thunder** version.

### Randomization criteria

The following criteria order is used (combinations producing empty presets are meant to be excluded):
- Enabled branches.
- Nations with any vehicles in the selected branch.
- Battle ratings with any vehicles filling the first two criteria.

Preset compositions are based on whether the main selected branch involves combined battles and which branches are enabled (except helicopters in Realistic or Simulator Battles where they require spawn points primarily earned by ground vehicles).

### GUI tips

- Hovering over a vehicle in the research tree highlights the one required to unlock said vehicle.
- Hovering over a vehicle in a preset highlights the former in the research tree.
- Clicking a vehicle in the preset opens the research tree tab it's on (if necessary) and scrolls it to bring the vehicle into view.

### User settings

*Client.Wpf.Settings.xml* stores user preferences. While it can be edited manually, it shouldn't be necessary to do so.

### Updating to new releases

New releases are shipped ready-to-go. The previous release should be deleted prior to extracting release files.

If you want to keep user preferences from the previous release, backup and copy *Client.Wpf.Settings.xml* from the previous release to the folder with the new release. Applicable settings would be carried over, use the backup to restore individual settings in case of conflicts.

### Road map

#### Pre-Alpha releases (reached)
- Setting up the framework.
- Prototyping.
- Visualisation of reseach trees with available vehicles.

#### Alpha releases (reached)
- Baseline randomisation.

#### Beta releases
- Extended randomisation: toggles for nations, countries (Australia, post-war Germany, Israel, etc.), branches, battle ratings, vehicle sub-types, individual vehicles, etc.
- Extended vehicle information: what is seen on vehicle cards in-game, and beyond.
- Aggregation of vehicle data.
- Improved presentation.

#### Production releases
- Visualisation of data changes between patches.
