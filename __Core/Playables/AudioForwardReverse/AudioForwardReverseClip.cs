using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class AudioForwardReverseClip : LerpToTargetClip
    {
        public new AudioForwardReverseBehaviour template = new AudioForwardReverseBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<AudioForwardReverseBehaviour>.Create(graph, template);
            return playable;
        }
    }
}