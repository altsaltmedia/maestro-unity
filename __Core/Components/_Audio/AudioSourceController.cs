using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace AltSalt
{
    public class AudioSourceController : MonoBehaviour
    {
        [SerializeField]
        GameObject audioPrefab;

        [SerializeField]
        List<AudioElement> audioElements = new List<AudioElement>();

        [SerializeField]
        EventPayloadKey loopAudioKey;

        [SerializeField]
        EventPayloadKey volumeKey;

        public void PlayAudioClip(EventPayload eventPayload)
        {
            AudioClipBundle audioClipBundle = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as AudioClipBundle;
            for (int i = 0; i < audioClipBundle.audioClipData.Count; i++) {

                bool loopClip = false;

                if (eventPayload.GetBoolValue(loopAudioKey) == true) {
                    loopClip = true;
                }

                float volume = 1f;

                if (Equals(eventPayload.GetFloatValue(volumeKey), -1f) == false) {
                    volume = eventPayload.GetFloatValue(volumeKey);
                }

                AudioElement audioElement = new AudioElement(audioPrefab, transform, audioClipBundle.audioClipData[i], loopClip, volume, audioClipBundle);
                audioElements.Add(audioElement);
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
