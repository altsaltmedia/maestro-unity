using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{
    [Serializable]
    public class AudioLerpSnapshotClip : LerpToTargetClip
    {
        public AudioLerpSnapshotBehaviour template = new AudioLerpSnapshotBehaviour();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<AudioLerpSnapshotBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
