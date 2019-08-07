using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class ResponsiveRectTransformScaleClip : ResponsiveLerpToTargetClip
    {
        public ResponsiveRectTransformScaleBehaviour template = new ResponsiveRectTransformScaleBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.parentTrack = parentTrack;
            template.clipAsset = this;

            var playable = ScriptPlayable<ResponsiveRectTransformScaleBehaviour>.Create (graph, template);
            return playable;
        }
    }   
}