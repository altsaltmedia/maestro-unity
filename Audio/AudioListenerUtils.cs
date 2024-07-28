using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Audio
{
    public class AudioListenerUtils : MonoBehaviour
    {
        [SerializeField]
        public float _initialVolume = 1;

        public float initialVolume
        {
            get => _initialVolume;
            set => _initialVolume = value;
        }

        private void Start()
        {
            AudioListener.volume = initialVolume;
        }

        public void ActivateVolume(GameObject callingObject)
        {
            AudioListener.volume = initialVolume;
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