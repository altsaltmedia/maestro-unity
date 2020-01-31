using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Audio
{
    public class AudioListenerUtils : MonoBehaviour
    {
        [SerializeField]
        [Required]
        [ReadOnly]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;

        private bool volumeActive
        {
            get => appSettings.GetVolumeEnabled(this.gameObject);
            set => appSettings.SetVolumeEnabled(this.gameObject, value);
        }
        
#if UNITY_EDITOR
        private void OnEnable()
        {
            _appSettings.PopulateVariable(this, nameof(_appSettings));
        }
#endif

        public void ToggleVoluem()
        {
            if (appSettings.GetVolumeEnabled(this.gameObject) == true) {
               DeactivateVolume();
            }
            else {
               ActivateVolume();
            }
        }
        
        public void ActivateVolume()
        {
            volumeActive = true;
            AudioListener.volume = 1;
        }

        public void DeactivateVolume()
        {
            volumeActive = false;
            AudioListener.volume = 0;
        }

    }
}