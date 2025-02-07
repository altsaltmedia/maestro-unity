﻿using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class FloatClip : LerpToTargetClip
    {
        [FormerlySerializedAs("template")]
        [SerializeField]
        private FloatBehaviour _template = new FloatBehaviour();

        public FloatBehaviour template
        {
            get => _template;
            set => _template = value;
        }
        
        public override LerpToTargetBehaviour templateReference => template;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.parentTrack = parentTrack;
            template.clipAsset = this;
            template.timelineInstanceConfig = timelineInstanceConfig;
            
            var playable = ScriptPlayable<FloatBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
