using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    public class MaestroConfig : ScriptableObject
    {
        [SerializeField]
        private string _projectAssetsPath = "Assets/__Project";

        public string projectAssetsPath => _projectAssetsPath;

        [SerializeField]
        private string _settingsPath = "Assets/AltSalt/MaestroConfig";

        public string settingsPath => _settingsPath;

        [SerializeField]
        private string _scriptsPath = "Assets/AltSalt/Maestro";

        public string scriptsPath => _scriptsPath;

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void RefreshAppSettings()
        {
            Utils.GetAppSettings();
        }
    }
}