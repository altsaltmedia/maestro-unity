using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class LerpVideoPlayerTimeClip : LerpToTargetClip
    {
        public new LerpVideoPlayerTimeBehaviour template = new LerpVideoPlayerTimeBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<LerpVideoPlayerTimeBehaviour>.Create(graph, template);
            return playable;
        }
    }
}