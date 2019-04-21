using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt
{
    [Serializable]
    public class ComplexEventTriggerClip : PlayableAsset, ITimelineClipAsset
    {
        public new ComplexEventTriggerBehaviour template = new ComplexEventTriggerBehaviour ();
        public double startTime;
        public double endTime;

        public ClipCaps clipCaps {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ComplexEventTriggerBehaviour>.Create(graph, template);
            ComplexEventTriggerBehaviour clone = playable.GetBehaviour();
            clone.startTime = startTime;
            clone.endTime = endTime;
            return playable;
        }
    }
}