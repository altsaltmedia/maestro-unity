using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{
    [Serializable]
    public class FloatClip : LerpToTargetClip
    {
        public FloatBehaviour template = new FloatBehaviour();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<FloatBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
