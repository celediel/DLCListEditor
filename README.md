# Yet Another DLCListEditor #

Simple GUI to generate/edit Grand Theft Auto 5's dlclist.xml files.

## Instructions ##

If you have GTA5 on Steam, installed in the default location, `C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V\`,
the program will automatically read your mods, `mods\update\x64\dlcpacks` and vanilla, `update\x64\dlcpacks`
dlcpack folders. Otherwise, select `File -> Select GTA5 Folder` and browse for your `GTA5.exe`.

This will populate the screen with all the dlcpacks you have installed, and will display whether they're located
in the vanilla folder, mods folder, or both. The fourth column allows you to choose whether the selected dlcpack
will get written to the xml or not. There's currently no sanity checking here, so it's probably best not to disable
any crucial dlcpacks. Once you've made your selection, select `File -> Save to new XML Document`, and choose where to save your file.

If you want to edit an existing dlclist.xml file, select `File -> Open existing XML Document` and browse for your XML document.
Once loaded, the screen, including the `Write to dlclist.xml?` column, will be updated to reflect the changes.
Loading another XML file will append it to your current list, rather than overwriting it.

If you make some unwanted changes, or load a funky XML file, or just want to start over,
 `File -> Clear current list` will clear everything, and let you optionally rescan your GTA directories.

It's also possible to open and save existing XML files without loading your GTA5 install.

## TODO ##

* Open from and write to update.rpf
* Anything else I think of that I want to do

## Changelog ##

* `1.0` - Initial release
* `1.01` - Added ReadMe redirection
* `1.02` - Added a status bar and some tooltips
