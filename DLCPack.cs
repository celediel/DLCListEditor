﻿namespace DLCListEditor
{
    class DLCPack
    {
        public DLCPack(string modName, bool inVanillaDir, bool inModsDir)
        {
            ModName = modName;
            InVanillaDir = inVanillaDir;
            InModsDir = inModsDir;
        }

        public DLCPack(string modName, bool inVanillaDir, bool inModsDir, bool inDlcList)
        {
            ModName = modName;
            InVanillaDir = inVanillaDir;
            InModsDir = inModsDir;
            InDlcList = inDlcList;
        }
        public string ModName { get; set; }
        public bool InVanillaDir { get; set; }
        public bool InModsDir { get; set; }
        public bool InDlcList { get; set; }
        public string InVanillaDirYesNo { get { return InVanillaDir ? "Yes" : "No"; } }
        public string InModsDirYesNo { get { return InModsDir ? "Yes" : "No"; } }
    }
}
