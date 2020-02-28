using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class ManualVideoPlayerTimeClip : LerpToTargetClip
    {
        [FormerlySerializedAs("template")]
        [SerializeField]
        private ManualVideoPlayerTimeBehaviour _template = new ManualVideoPlayerTimeBehaviour ();

        private ManualVideoPlayerTimeBehaviour template
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
            
            var playable = ScriptPlayable<ManualVideoPlayerTimeBehaviour>.Create(graph, template);
            return playable;
        }
    }
}