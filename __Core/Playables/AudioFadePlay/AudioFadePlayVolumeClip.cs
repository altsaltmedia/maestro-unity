using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class AudioFadePlayVolumeClip : LerpToTargetClip
    {
        public new AudioFadePlayVolumeBehaviour template = new AudioFadePlayVolumeBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<AudioFadePlayVolumeBehaviour>.Create(graph, template);
            return playable;
        }
    }
}