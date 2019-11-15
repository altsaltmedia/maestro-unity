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

        AudioSource oneShotHandler;

        void Start()
        {
            oneShotHandler = GetComponent<AudioSource>();
        }

        public void PlayAudioClip(EventPayload eventPayload)
        {
            AudioClipBundle audioClipBundle = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as AudioClipBundle;
            for (int i = 0; i < audioClipBundle.audioClipData.Count; i++) {

                bool loopClip = false;

                if (eventPayload.GetBoolValue(loopAudioKey) == true) {
                    loopClip = true;
                }

                float volume = 1f;

                if (float.IsNaN(eventPayload.GetFloatValue(volumeKey)) == false) {
                    volume = eventPayload.GetFloatValue(volumeKey);
                }

                AudioElement audioElement = new AudioElement(audioPrefab, transform, audioClipBundle.audioClipData[i], loopClip, volume, audioClipBundle);
                audioElements.Add(audioElement);
            }
        }

        public void PlayOneShot(EventPayload eventPayload)
        {
            AudioClipBundle audioClipBundle = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as AudioClipBundle;
            for (int i = 0; i < audioClipBundle.audioClipData.Count; i++) {

                float volume = 1f;

                if (eventPayload.GetBoolValue(DataType.boolType) == true) {
                    volume = UnityEngine.Random.Range(.25f, 1f);
                }

                oneShotHandler.PlayOneShot(audioClipBundle.audioClipData[i].audioClip, volume);
            }
        }

        public void FadeOutAudioClips(EventPayload eventPayload)
        {
            AudioClipBundle audioClipBundle = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as AudioClipBundle;

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

            public AudioElement(GameObject audioPrefab, Transform parent, AudioClipData audioClipData, bool loop, float volume, AudioClipBundle referenceBundle)
            {
                audioSourceObject = Instantiate(audioPrefab);
                audioSourceObject.transform.SetParent(parent.transform);

                audioSource = audioSourceObject.GetComponent<AudioSource>();
                audioSource.clip = audioClipData.audioClip;
                audioSource.outputAudioMixerGroup = audioClipData.targetMixerGroup;
                audioSource.loop = loop;
                audioSource.volume = volume;
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
