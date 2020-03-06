using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    public class AudioLerpSnapshotClip : LerpToTargetClip
    {
        [FormerlySerializedAs("template")]
        [SerializeField]
        private AudioLerpSnapshotBehaviour _template = new AudioLerpSnapshotBehaviour();

        public AudioLerpSnapshotBehaviour template
        {
            get => _template;
            set => _template = value;
        }
        
        public override LerpToTargetBehaviour templateReference => template;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<AudioLerpSnapshotBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
