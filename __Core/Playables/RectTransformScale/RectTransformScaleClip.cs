using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class RectTransformScaleClip : LerpToTargetClip
    {
        public new RectTransformScaleBehaviour template = new RectTransformScaleBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<RectTransformScaleBehaviour>.Create (graph, template);
            return playable;
        }
    }   
}