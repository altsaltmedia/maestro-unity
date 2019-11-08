﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Audio Clip Bundle")]
    public class AudioClipBundle : RegisterableScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Audio Clip Bundle")]
        string description = "";
#endif
        [SerializeField]
        public List<AudioClipData> audioClipData = new List<AudioClipData>();
    }

    [Serializable]
    public class AudioClipData
    {
        [SerializeField]
        public AudioClip audioClip;

        [SerializeField]
        public AudioMixerGroup targetMixerGroup;

    }

}