using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class LerpVideoPlayerTimeClip : LerpToTargetClip
    {
        [FormerlySerializedAs("template")]
        [SerializeField]
        private LerpVideoPlayerTimeBehaviour _template = new LerpVideoPlayerTimeBehaviour ();

        private LerpVideoPlayerTimeBehaviour template
        {
            get => _template;
            set => _template = value;
        }

        public override LerpToTargetBehaviour templateReference => template;

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.parentTrack = parentTrack;
            template.clipAsset = this;
            template.timelineInstanceConfig = timelineInstanceConfig;
            
            var playable = ScriptPlayable<LerpVideoPlayerTimeBehaviour>.Create(graph, template);
            return playable;
        }
    }
}