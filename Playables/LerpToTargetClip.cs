/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [Serializable]
    public abstract class LerpToTargetClip : PlayableAsset, ITimelineClipAsset {

        // This is here for implementation reference only. DO NOT attempt to use or access this when
        // creating subclasses; it won't work. Instead, create a new instance of your behaviour in question.
        // private LerpToTargetBehaviour template = new LerpToTargetBehaviour();

        public abstract LerpToTargetBehaviour templateReference { get; }
        
        [ShowInInspector]
        private double _startTime;

        public double startTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        [ShowInInspector]
        private double _endTime;
        
        public double endTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        [HideInInspector]
        public TrackAsset _parentTrack;

        public TrackAsset parentTrack
        {
            get => _parentTrack;
            set => _parentTrack = value;
        }

        [HideInInspector]
        public TrackAssetConfig _trackAssetConfig;

        public TrackAssetConfig trackAssetConfig
        {
            get => _trackAssetConfig;
            set => _trackAssetConfig = value;
        }

        public override double duration => 1d;

        public ClipCaps clipCaps => ClipCaps.None;

//        This is here for reference - it should be overriden in your child class
//        so that the instance of your behaviour has all the necessary variables.
//
//        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
//        {
//
//            template.startTime = startTime;
//            template.endTime = endTime;
//            template.parentTrack = parentTrack;
//            template.clipAsset = this;
//            template.directorObject = directorObject;
//
//            var playable = ScriptPlayable<LerpToTargetBehaviour>.Create(graph, template);
//            return playable;
//        }
    }
}