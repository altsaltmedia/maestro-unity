using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class DebugTimelineClip : LerpToTargetClip
    {
        public new DebugTimelineBehaviour template = new DebugTimelineBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<DebugTimelineBehaviour>.Create(graph, template);
            return playable;
        }
    }
}