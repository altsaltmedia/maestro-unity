using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Audio
{
    public class AudioListenerUtils : MonoBehaviour
    {
        public void ActivateVolume(GameObject callingObject)
        {
            AudioListener.volume = 1;
        }

        public void DeactivateVolume(GameObject callingObject)
        {
            AudioListener.volume = 0;
        }

        public void SetVolume(FloatVariable floatVariable)
        {
            AudioListener.volume = floatVariable.value;
        }
    }
}   