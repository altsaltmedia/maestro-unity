using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ComplexEventTimelineTriggerClip : TimelineTriggerClip
    {
        public ComplexEventTimelineTriggerBehaviour template = new ComplexEventTimelineTriggerBehaviour ();

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.trackAssetConfig = trackAssetConfig;

            var playable = ScriptPlayable<ComplexEventTimelineTriggerBehaviour>.Create(graph, template);
            return playable;
        }
    }
}