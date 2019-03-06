/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt
{
    [Serializable]
    public class LerpToTargetClip : PlayableAsset, ITimelineClipAsset {
        
        public LerpToTargetBehaviour template = new LerpToTargetBehaviour();
        public double startTime;
        public double endTime;

        public override double duration {
            get {
                return 1d;
            }
        }

        public ClipCaps clipCaps {
            get { return ClipCaps.None; }
        }
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<LerpToTargetBehaviour>.Create(graph, template);
            return playable;
        }
    }
}