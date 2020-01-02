/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class TMProTypewriterClip : LerpToTargetClip {
        
        [FormerlySerializedAs("template")]
        [SerializeField]
        private TMProTypewriterBehaviour _template = new TMProTypewriterBehaviour();

        private TMProTypewriterBehaviour template
        {
            get => _template;
            set => _template = value;
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.parentTrack = parentTrack;
            template.clipAsset = this;
            template.directorObject = directorObject;
            
            var playable = ScriptPlayable<TMProTypewriterBehaviour>.Create(graph, template);
            return playable;
        }
    }
}