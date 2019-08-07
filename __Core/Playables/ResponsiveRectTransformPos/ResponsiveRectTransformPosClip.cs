using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class ResponsiveRectTransformPosClip : ResponsiveLerpToTargetClip
    {
        public ResponsiveRectTransformPosBehaviour template = new ResponsiveRectTransformPosBehaviour();

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.parentTrack = parentTrack;
            template.clipAsset = this;

            var playable = ScriptPlayable<ResponsiveRectTransformPosBehaviour>.Create (graph, template);
            return playable;
        }
    }   
}