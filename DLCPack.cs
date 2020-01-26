using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLCListEditor
{
    class DLCPack
    {
        public DLCPack(string modName, bool inVanillaDir, bool inModsDir)
        {
            ModName = modName;
            InVanillaDir = inVanillaDir;
            InModsDir = inModsDir;
        }
        public string ModName { get; set; }
        public bool InVanillaDir { get; set; }
        public bool InModsDir { get; set; }
        public bool InDlcList { get; set; }
    }
}
