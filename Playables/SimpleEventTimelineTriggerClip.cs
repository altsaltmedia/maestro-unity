using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [Serializable]
    public class SimpleEventTimelineTriggerClip : TimelineTriggerClip
    {
        public SimpleEventTimelineTriggerBehaviour template = new SimpleEventTimelineTriggerBehaviour ();

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.trackAssetConfig = trackAssetConfig;

            var playable = ScriptPlayable<SimpleEventTimelineTriggerBehaviour>.Create(graph, template);
            return playable;
        }
    }
}