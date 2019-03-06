using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class FadePlayAudioVolumeClip : LerpToTargetClip
    {
        public new FadePlayAudioVolumeBehaviour template = new FadePlayAudioVolumeBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<FadePlayAudioVolumeBehaviour>.Create(graph, template);
            return playable;
        }
    }
}