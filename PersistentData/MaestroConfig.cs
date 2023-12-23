using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "Maestro/Maestro Config")]
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

#if UNITY_EDITOR 
        private void OnEnable()
        {
            var appSettingsSearch = Utils.GetScriptableObject(nameof(AppSettings));
            if (appSettingsSearch == null) {
                AppSettings appSettings = Utils.GetAppSettings();
                appSettings.RefreshDependencies();
                appSettings.SetDefaults();
            }
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [MenuItem("Tools/Maestro/Reset App Settings")]
        public static void ResetAppSettings()
        {
            AppSettings appSettings = Utils.GetAppSettings();
            appSettings.RefreshDependencies();
            appSettings.SetDefaults();
        }
#endif   
        
    }
}