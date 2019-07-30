using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class TMProCharSpacingClip : LerpToTargetClip
    {
        public new TMProCharSpacingBehaviour template = new TMProCharSpacingBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<TMProCharSpacingBehaviour>.Create (graph, template);
            return playable;
        }
    }   
}