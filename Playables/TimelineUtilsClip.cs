using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class TimelineUtilsClip : LerpToTargetClip
    {
        [FormerlySerializedAs("template")]
        [SerializeField]
        private TimelineUtilsBehaviour _template = new TimelineUtilsBehaviour ();

        private TimelineUtilsBehaviour template
        {
            get => _template;
            set => _template = value;
        }

        public override LerpToTargetBehaviour templateReference => template;
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.trackAssetConfig = trackAssetConfig;
            
            var playable = ScriptPlayable<TimelineUtilsBehaviour>.Create(graph, template);
            return playable;
        }
    }
}