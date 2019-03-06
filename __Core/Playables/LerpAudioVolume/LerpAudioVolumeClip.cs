using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class LerpAudioVolumeClip : LerpToTargetClip
    {
        public new LerpAudioVolumeBehaviour template = new LerpAudioVolumeBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<LerpAudioVolumeBehaviour>.Create(graph, template);
            return playable;
        }
    }
}