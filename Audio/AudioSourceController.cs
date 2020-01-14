using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace AltSalt.Maestro.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceController : MonoBehaviour
    {
        [SerializeField]
        GameObject audioPrefab;

        [SerializeField]
        List<AudioElement> audioElements = new List<AudioElement>();

        [SerializeField]
        CustomKey loopAudioKey;

        [SerializeField]
        CustomKey volumeKey;
        
        [SerializeField]
        CustomKey pitchKey;

        AudioSource oneShotHandler;

        void Start()
        {
            oneShotHandler = GetComponent<AudioSource>();
        }

        public void PlayAudioClip(ComplexPayload complexPayload)
        {
            AudioClipBundle audioClipBundle = complexPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as AudioClipBundle;
            for (int i = 0; i < audioClipBundle.audioClipData.Count; i++) {

                bool loopClip = complexPayload.GetBoolValue(loopAudioKey) == true;

                float volume = 1f;

                if (float.IsNaN(complexPayload.GetFloatValue(volumeKey)) == false) {
                    volume = complexPayload.GetFloatValue(volumeKey);
                }
                
                float pitch = 1f;

                if (float.IsNaN(complexPayload.GetFloatValue(pitchKey)) == false) {
                    volume = complexPayload.GetFloatValue(pitchKey);
                }

                AudioElement audioElement = new AudioElement(audioPrefab, transform, audioClipBundle.audioClipData[i], loopClip, volume, pitch, audioClipBundle);
                audioElements.Add(audioElement);
            }
        }

        public void PlayOneShot(ComplexPayload complexPayload)
        {
            AudioClipBundle audioClipBundle = complexPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as AudioClipBundle;
            for (int i = 0; i < audioClipBundle.audioClipData.Count; i++) {

                float volume = 1f;

                if (float.IsNaN(complexPayload.GetFloatValue(volumeKey)) == false) {
                    volume = complexPayload.GetFloatValue(volumeKey);
                }
                
                oneShotHandler.PlayOneShot(audioClipBundle.audioClipData[i].audioClip, volume);
            }
        }

        public void FadeOutAudioClips(ComplexPayload complexPayload)
        {
            AudioClipBundle audioClipBundle = complexPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as AudioClipBundle;

            for(int i=0; i<audioElements.Count; i++) {
                if(audioElements[i].audioClipBundle == audioClipBundle) {
                    audioElements[i].FadeOut();
                }
            }
        }

        [Serializable]
        class AudioElement
        {
            GameObject audioSourceObject;
            public AudioSource audioSource;
            public AudioClipBundle audioClipBundle;

            public AudioElement(GameObject audioPrefab, Transform parent, AudioClipData audioClipData, bool loop, float volume, float pitch, AudioClipBundle referenceBundle)
            {
                audioSourceObject = Instantiate(audioPrefab);
                audioSourceObject.transform.SetParent(parent.transform);

                audioSource = audioSourceObject.GetComponent<AudioSource>();
                audioSource.clip = audioClipData.audioClip;
                audioSource.outputAudioMixerGroup = audioClipData.targetMixerGroup;
                audioSource.loop = loop;
                audioSource.volume = volume;
                audioSource.pitch = pitch;
                audioSource.Play();
                audioClipBundle = referenceBundle;
            }

            public void FadeOut()
            {
                DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0, 5f).OnComplete(StopAudio);
            }

            void StopAudio()
            {
                audioSource.Stop();
            }
        
        }

    }

}
