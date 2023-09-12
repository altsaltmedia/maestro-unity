using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

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
            for (int i = 0; i < audioClipBundle.audioClipDataList.Count; i++) {

                bool loopClip = complexPayload.GetBoolValue(loopAudioKey) == true;

                float volume = 1f;

                if (float.IsNaN(complexPayload.GetFloatValue(volumeKey)) == false) {
                    volume = complexPayload.GetFloatValue(volumeKey);
                }
                
                float pitch = 1f;

                if (float.IsNaN(complexPayload.GetFloatValue(pitchKey)) == false) {
                    volume = complexPayload.GetFloatValue(pitchKey);
                }

                // Do not play the clip if it's already been registered
                if(!audioElements.Exists( x => x.audioClipBundle == audioClipBundle)) {
                    AudioElement audioElement = new AudioElement(audioPrefab, transform, audioClipBundle.audioClipDataList[i], loopClip, volume, pitch, audioClipBundle);
                    audioElements.Add(audioElement);
                }
            }
        }

        public void PlayOneShot(ComplexPayload complexPayload)
        {
            AudioClipBundle audioClipBundle = complexPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as AudioClipBundle;
            for (int i = 0; i < audioClipBundle.audioClipDataList.Count; i++) {

                float volume = 1f;

                if (float.IsNaN(complexPayload.GetFloatValue(volumeKey)) == false) {
                    volume = complexPayload.GetFloatValue(volumeKey);
                }
                
                oneShotHandler.PlayOneShot(audioClipBundle.audioClipDataList[i].audioClip, volume);
            }
        }
        
        public void PlayOneShotOverride(ComplexPayload complexPayload)
        {
            oneShotHandler.Stop();
            PlayOneShot(complexPayload);
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

        public void CallFadeOutAndRemoveAudioClips(ComplexPayload complexPayload) {
            StartCoroutine(FadeOutAndRemoveAudioClips(complexPayload));
        }


        private IEnumerator FadeOutAndRemoveAudioClips(ComplexPayload complexPayload)
        {
            AudioClipBundle audioClipBundle = complexPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as AudioClipBundle;

            for (int i = 0; i < audioElements.Count; i++)
            {
                if (audioElements[i].audioClipBundle == audioClipBundle)
                {
                    yield return StartCoroutine(audioElements[i].FadeOut());
                }
            }

            for (int i = audioElements.Count - 1; i >= 0; i--)
            {    
                if (audioElements[i].audioClipBundle == audioClipBundle)
                {
                    Destroy(audioElements[i].audioSource.gameObject);
                    audioElements.RemoveAt(i);
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

            public IEnumerator FadeOut(Action callback = null)
            {
                bool isFadingOut = true;

                DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0, 1f).OnComplete(() => {
                    StopAudio();
                    isFadingOut = false;
                });

                while (isFadingOut == true) {
                    yield return null;
                }
            }

            void StopAudio()
            {
                audioSource.Stop();
            }
        
        }

    }

}
