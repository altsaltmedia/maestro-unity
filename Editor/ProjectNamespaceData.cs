using System.Collections.Generic;

namespace AltSalt.Maestro
{
    public struct ProjectNamespaceData
    {
        private static readonly Dictionary<ModuleNamespace, ModuleNamespaceStrings> _namespaceData = new Dictionary<ModuleNamespace, ModuleNamespaceStrings>
        {
            { ModuleNamespace.Root, new ModuleNamespaceStrings( Utils.rootNamespace, Utils.scriptsPath + "//") },
            { ModuleNamespace.Layout, new ModuleNamespaceStrings( Utils.rootNamespace + ".Layout", Utils.scriptsPath + "/Layout/")},
            { ModuleNamespace.Sequencing, new ModuleNamespaceStrings( Utils.rootNamespace + ".Sequencing", Utils.scriptsPath + "/Sequencing/")},
            { ModuleNamespace.Autorun, new ModuleNamespaceStrings( Utils.rootNamespace + ".Sequencing.Autorun", Utils.scriptsPath + "/Sequencing/Autorun/")},
            { ModuleNamespace.Touch, new ModuleNamespaceStrings( Utils.rootNamespace + ".Sequencing.Touch", Utils.scriptsPath + "/Sequencing/Touch/")},
            { ModuleNamespace.Animation, new ModuleNamespaceStrings( Utils.rootNamespace + ".Animation",Utils.scriptsPath + "/Animation/")},
            { ModuleNamespace.Audio, new ModuleNamespaceStrings( Utils.rootNamespace + ".Audio", Utils.scriptsPath + "/Audio/")},
            { ModuleNamespace.Logic, new ModuleNamespaceStrings( Utils.rootNamespace + ".Logic", Utils.scriptsPath + "/Logic/")},
            { ModuleNamespace.Sensors, new ModuleNamespaceStrings( Utils.rootNamespace + ".Sensors", Utils.scriptsPath + "/Sensors/")}
        };

        public static Dictionary<ModuleNamespace, ModuleNamespaceStrings> namespaceData => _namespaceData;
    }
}