using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AltSalt.Maestro.Audio
{
    public class AudioListenerUtils : MonoBehaviour
    {

        public void ActivateVolume()
        {
            AudioListener.volume = 1;
        }

        public void DeactivateVolume()
        {
            AudioListener.volume = 0;
        }

    }
}