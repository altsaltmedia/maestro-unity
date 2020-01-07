using UnityEngine;

namespace AltSalt.Maestro.Audio
{
    [ExecuteInEditMode]
    public class AudioSourceUtils : MonoBehaviour
    {
        
#if UNITY_EDITOR
        [SerializeField]
        [Multiline]
        private string _description;
#endif
    
        private AudioSource _audioSource;

        private AudioSource audioSource
        {
            get => _audioSource;
            set => _audioSource = value;
        }

        private void Start ()
        {
            GetAudioSource();
        }

        private void GetAudioSource() {
            if (audioSource == null) {
                audioSource = GetComponent<AudioSource>();
            }
        }

        public void SetPitch(FloatVariable floatVariable)
        {
            GetAudioSource();
            audioSource.pitch = floatVariable.value;
        }
        
        public void SetVolume(FloatVariable floatVariable)
        {
            GetAudioSource();
            audioSource.volume = floatVariable.value;
        }
    }
}