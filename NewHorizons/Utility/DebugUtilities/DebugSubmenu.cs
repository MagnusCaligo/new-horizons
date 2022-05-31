using NewHorizons.External.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewHorizons.Utility.DebugUtilities
{
    abstract class DebugSubmenu
    {
        internal abstract void OnAwake(DebugMenu menu);
        internal abstract void OnGUI(DebugMenu menu);
        internal abstract void PreSave(DebugMenu menu);
        internal abstract void OnInit(DebugMenu menu);
        internal abstract void LoadConfigFile(DebugMenu menu, PlanetConfig config);

        internal abstract string SubmenuName();

    }
}
