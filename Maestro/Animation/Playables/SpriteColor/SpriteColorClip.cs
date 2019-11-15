using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class SpriteColorClip : LerpToTargetClip
    {
        public new SpriteColorBehaviour template = new SpriteColorBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<SpriteColorBehaviour>.Create (graph, template);
            return playable;
        }
    }   
}