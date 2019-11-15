using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class TMProColorClip : LerpToTargetClip
    {
        public new TMProColorBehaviour template = new TMProColorBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<TMProColorBehaviour>.Create (graph, template);
            return playable;
        }
    }   
}