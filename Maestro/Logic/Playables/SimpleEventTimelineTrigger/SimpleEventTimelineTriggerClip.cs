using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [Serializable]
    public class SimpleEventTimelineTriggerClip : RegisterablePlayableAsset, ITimelineClipAsset
    {
        public SimpleEventTimelineTriggerBehaviour template = new SimpleEventTimelineTriggerBehaviour ();
        public double startTime;
        public double endTime;

        public BoolReference isReversing;

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
            template.isReversing.Variable = isReversing.Variable;

            var playable = ScriptPlayable<SimpleEventTimelineTriggerBehaviour>.Create(graph, template);
            return playable;
        }
    }
}