using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class LerpColorVarClip : LerpToTargetClip
    {
        public new LerpColorVarBehaviour template = new LerpColorVarBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<LerpColorVarBehaviour>.Create(graph, template);
            return playable;
        }
    }
}