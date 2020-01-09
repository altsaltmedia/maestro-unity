using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AltSalt.Maestro.Audio
{
    public class AudioListenerUtils : MonoBehaviour
    {
        [SerializeField]
        private AppSettings _appSettings;

        private AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }
                return _appSettings;
            }
            set => _appSettings = value;
        }

        private static bool _volumeActive = true;

        private static bool volumeActive
        {
            get => _volumeActive;
            set => _volumeActive = value;
        }

        private void Start()
        {
            
        }

        public void ToggleVoluem()
        {
            if (appSettings.volumeEnabled == true) {
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