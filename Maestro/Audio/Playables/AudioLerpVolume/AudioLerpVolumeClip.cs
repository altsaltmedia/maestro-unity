using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    public class AudioLerpVolumeClip : LerpToTargetClip
    {
        public AudioLerpVolumeBehaviour template = new AudioLerpVolumeBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<AudioLerpVolumeBehaviour>.Create(graph, template);
            return playable;
        }
    }
}