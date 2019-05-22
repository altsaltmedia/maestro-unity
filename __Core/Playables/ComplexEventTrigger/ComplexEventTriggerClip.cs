using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt
{
    [Serializable]
    public class ComplexEventTriggerClip : PlayableAsset, ITimelineClipAsset
    {
        public ComplexEventTriggerBehaviour template = new ComplexEventTriggerBehaviour ();
        public double startTime;
        public double endTime;

        public override double duration {
            get {
                return 1d;
            }
        }

        public ClipCaps clipCaps {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<ComplexEventTriggerBehaviour>.Create(graph, template);
            return playable;
        }
    }
}