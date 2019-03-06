using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class RectTransformPosClip : LerpToTargetClip
    {
        public new RectTransformPosBehaviour template = new RectTransformPosBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<RectTransformPosBehaviour>.Create (graph, template);
            return playable;
        }
    }   
}