using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    public class AudioFadePlayVolumeClip : LerpToTargetClip
    {
        [FormerlySerializedAs("template")]
        [SerializeField]
        private AudioFadePlayVolumeBehaviour _template = new AudioFadePlayVolumeBehaviour();

        private AudioFadePlayVolumeBehaviour template
        {
            get => _template;
            set => _template = value;
        }

        [FormerlySerializedAs("targetAudioSources")]
        [SerializeField]
        private List<ExposedAudioSource> _targetAudioSources = new List<ExposedAudioSource>();

        private List<ExposedAudioSource> targetAudioSources
        {
            get => _targetAudioSources;
            set => _targetAudioSources = value;
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.targetAudioSources.Clear();

            for (int i = 0; i < targetAudioSources.Count; i++) {
                AudioSource audioObject = targetAudioSources[i].audioSource.Resolve(graph.GetResolver());
                if(audioObject != null) {
                    template.targetAudioSources.Add(audioObject);
                }
            }

            var playable = ScriptPlayable<AudioFadePlayVolumeBehaviour>.Create(graph, template);
            return playable;
        }
    }

    [Serializable]
    public class ExposedAudioSource
    {
        [FormerlySerializedAs("audioSource")]
        [SerializeField]
        private ExposedReference<AudioSource> _audioSource;

        public ExposedReference<AudioSource> audioSource
        {
            get => _audioSource;
            set => _audioSource = value;
        }
    }

}