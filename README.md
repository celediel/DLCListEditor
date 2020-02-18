# Yet Another DLCListEditor #

Simple GUI to generate/edit Grand Theft Auto 5's (and maybe Red Dead Redemption 2's) dlclist.xml files.

## Instructions ##

Select `File -> Select Game Folder` and browse for your `GTA5.exe` or `RDR2.exe`,
the program will automatically read your mods, and vanilla dlcpack folders.

The screen will be populated with all the dlcpacks you have installed, and will display whether they're located
in the vanilla folder, mods folder, or both. The fourth column allows you to choose whether the selected dlcpack
will get written to the xml or not. There's currently no sanity checking here, so it's probably best not to disable
any crucial dlcpacks. Once you've made your selection, select `File -> Save to new XML Document`, and choose where to save your file.

If you want to edit an existing dlclist.xml file, select `File -> Open existing XML Document` and browse for your XML document.
Once loaded, the screen, including the `Write to dlclist.xml?` column, will be updated to reflect the changes.
Loading another XML file will append it to your current list, rather than overwriting it.

If you make some unwanted changes, or load a funky XML file, or just want to start over,
 `File -> Clear current list` will clear everything, and let you optionally rescan your GTA directories.

It's also possible to open and save existing XML files without loading any game install.

## Options / Config ##

There now exists a `config.ini` that currently contains only one option:

    [Paths]
    GTA5 = C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V\

If it doesn't exist, it'll be created the first time a directory is selected, if
`Options -> Automatically save directory to config.ini?` is checked, so don't worry about manually
creating the file.

## TODO ##

* ~~Open from and write to update.rpf~~ As far as I can tell, CodeWalkers' implementation of RPF file loading
depends on WinForms (seriously?), and I'm using WPF for this program, so I'm putting this feature on hold for now :(
* Anything else I think of that I want to do
* Proper Red Dead Redemption 2 support (when someone releases something that edits RDR2's rpfs)

## Changelog ##

* `1.0` - Initial release
* `1.01` - Added ReadMe redirection
* `1.02` - Added a status bar and some tooltips
* `1.1` - Added a config.ini that for now only contains one option, under `[Paths]`, `GTA5 = C:\Path\To\GTA5`.
In addition, `Help -> View Readme` now actually displays the readme in a window instead of launching a browser to gitlab.
* `1.11` - Migrated to Github, also added a "proper" icon so it doesen't look super janky.
Also some minor code refactoring and other nonsense.
* `1.2` - Getting ready for RDR 2 support. Currently correctly parses RDR2's vanilla dlcpack structure,
and (hopefully) all hardcoded GTA5 references have been removed.

## Credits ##

* rickyah's ini-parser - <https://github.com/rickyah/ini-parser>
* Kryptos-FR's Markdig-WPF for Markdown to WPF used for displaying readme - <https://github.com/Kryptos-FR/markdig.wpf>
* Nate Shoffner & Evan Wondrasek / Apricity Software LLC for WPFCustomMessageBox - <https://github.com/NateShoffner/WPFCustomMessageBox>
