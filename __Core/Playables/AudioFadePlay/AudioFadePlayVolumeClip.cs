using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class AudioFadePlayVolumeClip : LerpToTargetClip
    {
        public AudioFadePlayVolumeBehaviour template = new AudioFadePlayVolumeBehaviour();

        public List<ExposedAudioSource> targetAudioSources = new List<ExposedAudioSource>();

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
        public ExposedReference<AudioSource> audioSource;
    }

}