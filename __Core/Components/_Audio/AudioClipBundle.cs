using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Audio Clip Bundle")]
    public class AudioClipBundle : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Audio Clip Bundle")]
        string description = "";
#endif

        [SerializeField]
        public List<AudioClip> audioClips = new List<AudioClip>();

    }
}