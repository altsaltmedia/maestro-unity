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

        public void PlayAudioClip(EventPayload eventPayload)
        {
            AudioClipBundle audioClipBundle = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as AudioClipBundle;
            for (int i = 0; i < audioClipBundle.audioClips.Count; i++) {

                bool loopClip = false;

                if (eventPayload.GetBoolValue(loopAudioKey) == true) {
                    loopClip = true;
                }

                AudioElement audioElement = new AudioElement(audioPrefab, transform, audioClipBundle.audioClips[i], loopClip, audioClipBundle);
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

            public AudioElement(GameObject audioPrefab, Transform parent, AudioClip audioClip, bool loop, AudioClipBundle referenceBundle)
            {
                audioSourceObject = Instantiate(audioPrefab);
                audioSourceObject.transform.SetParent(parent.transform);

                audioSource = audioSourceObject.GetComponent<AudioSource>();
                audioSource.clip = audioClip;
                audioSource.loop = loop;
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
