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
    public class TMProTypewriterClip : LerpToTargetClip {
        
        public new TMProTypewriterBehaviour template = new TMProTypewriterBehaviour();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<TMProTypewriterBehaviour>.Create(graph, template);
            return playable;
        }
    }
}