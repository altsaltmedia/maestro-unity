using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class RectTransformRotationClip : LerpToTargetClip
    {
        public new RectTransformRotationBehaviour template = new RectTransformRotationBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<RectTransformRotationBehaviour>.Create (graph, template);
            return playable;
        }
    }   
}