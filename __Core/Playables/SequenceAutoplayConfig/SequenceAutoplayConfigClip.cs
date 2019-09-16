using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class SequenceAutoplayConfigClip : LerpToTargetClip
    {
        public SequenceAutoplayConfigBehaviour template = new SequenceAutoplayConfigBehaviour();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;

            var playable = ScriptPlayable<SequenceAutoplayConfigBehaviour>.Create(graph, template);
            return playable;
        }
    }
}