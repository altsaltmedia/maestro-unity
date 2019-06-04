using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt {

    [ExecuteInEditMode]
    public class PlayableAudioSource : MonoBehaviour {
        
        AudioSource audioSource;

        void Start ()
        {
            GetAudioSource();
        }

        void GetAudioSource() {
            audioSource = GetComponent<AudioSource>();
        }

#if UNITY_EDITOR
        [Button(ButtonSizes.Large), GUIColor(0.1f, 0.8f, 0.1f)]
        [InfoBox("Restarts the audio track.")]
        public void PlayTrack()
        {
            if(audioSource == null) {
                GetAudioSource();
            }

            audioSource.Stop();
            audioSource.Play();
        }

        [Button(ButtonSizes.Large), GUIColor(0.6f, 0.3f, 0.3f)]
        [InfoBox("Stops the audio track.")]
        public void StopTrack()
        {
            if (audioSource == null) {
                GetAudioSource();
            }

            audioSource.Stop();
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.8f, 0.1f)]
        [InfoBox("Sets track volume to 1.")]
        public void ResetVolume()
        {
            if (audioSource == null) {
                GetAudioSource();
            }

            audioSource.volume = 1;
        }

        [Button(ButtonSizes.Large), GUIColor(0.5f, 0.5f, 0.9f)]
        [InfoBox("Sets track volume to 0.")]
        public void Mute()
        {
            if (audioSource == null) {
                GetAudioSource();
            }

            audioSource.volume = 0;
        }
#endif

    }

}