using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class SequencePauseMomentumConfigClip : LerpToTargetClip
    {
        public SequencePauseMomentumConfigBehaviour template = new SequencePauseMomentumConfigBehaviour();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;

            var playable = ScriptPlayable<SequencePauseMomentumConfigBehaviour>.Create(graph, template);
            return playable;
        }
    }
}