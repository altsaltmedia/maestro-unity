using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{
    [Serializable]
    public class LerpFloatVarClip : LerpToTargetClip
    {
        public new LerpFloatVarBehaviour template = new LerpFloatVarBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<LerpFloatVarBehaviour>.Create(graph, template);
            return playable;
        }
    }
}