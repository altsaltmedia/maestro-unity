using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Audio
{
    [CreateAssetMenu(menuName = "Maestro/Audio/Audio Clip Bundle", order = 0)]
    public class AudioClipBundle : RegisterableScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Audio Clip Bundle")]
        string description = "";
#endif
        [FormerlySerializedAs("audioClipData"),SerializeField]
        public List<AudioClipData> audioClipDataList = new List<AudioClipData>();
    }

    [Serializable]
    public class AudioClipData
    {
        [SerializeField]
        public AudioClip audioClip;

        [SerializeField]
        public AudioMixerGroup targetMixerGroup;

        public AudioClipData(AudioClip audioClip)
        {
            this.audioClip = audioClip;
        }
    }

}