using System.Collections.Generic;

namespace AltSalt.Maestro
{
    public struct ProjectNamespaceData
    {
        private static readonly Dictionary<ModuleNamespace, ModuleNamespaceStrings> _namespaceData = new Dictionary<ModuleNamespace, ModuleNamespaceStrings>
        {
            { ModuleNamespace.Root, new ModuleNamespaceStrings("AltSalt.Maestro", "Assets/_AltSalt/Maestro/") },
            { ModuleNamespace.Layout, new ModuleNamespaceStrings( "AltSalt.Maestro.Layout", "Assets/_AltSalt/Maestro/Layout/")},
            { ModuleNamespace.Modify, new ModuleNamespaceStrings( "AltSalt.Maestro.Layout.Modify", "Assets/_AltSalt/Maestro/Layout/Modify/")},
            { ModuleNamespace.Sequencing, new ModuleNamespaceStrings( "AltSalt.Maestro.Sequencing", "Assets/_AltSalt/Maestro/Sequencing/")},
            { ModuleNamespace.Autorun, new ModuleNamespaceStrings( "AltSalt.Maestro.Sequencing.Autorun", "Assets/_AltSalt/Maestro/Sequencing/Autorun/")},
            { ModuleNamespace.Touch, new ModuleNamespaceStrings( "AltSalt.Maestro.Sequencing.Touch", "Assets/_AltSalt/Maestro/Sequencing/Touch/")},
            { ModuleNamespace.Animation, new ModuleNamespaceStrings( "AltSalt.Maestro.Animation", "Assets/_AltSalt/Maestro/Animation/")},
            { ModuleNamespace.Audio, new ModuleNamespaceStrings( "AltSalt.Maestro.Audio", "Assets/_AltSalt/Maestro/Audio/")},
            { ModuleNamespace.Logic, new ModuleNamespaceStrings( "AltSalt.Maestro.Logic", "Assets/_AltSalt/Maestro/Logic/")},
            { ModuleNamespace.Sensors, new ModuleNamespaceStrings( "AltSalt.Maestro.Sensors", "Assets/_AltSalt/Maestro/Sensors/")}
        };

        public static Dictionary<ModuleNamespace, ModuleNamespaceStrings> namespaceData => _namespaceData;
    }
}