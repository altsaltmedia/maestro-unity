using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt
{
    [Serializable]
    public class SimpleEventTriggerClip : PlayableAsset, ITimelineClipAsset
    {
        public new SimpleEventTriggerBehaviour template = new SimpleEventTriggerBehaviour ();
        public double startTime;
        public double endTime;

        public ClipCaps clipCaps {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SimpleEventTriggerBehaviour>.Create(graph, template);
            SimpleEventTriggerBehaviour clone = playable.GetBehaviour();
            clone.startTime = startTime;
            clone.endTime = endTime;
            return playable;
        }
    }
}