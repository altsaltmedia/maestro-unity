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
    public abstract class ResponsiveLerpToTargetClip : PlayableAsset, ITimelineClipAsset {

        // This is here for implementation reference only. DO NOT attempt to use or access this when
        // creating subclasses; it won't work. Instead, create a new instance of your behaviour in question.
        private ResponsiveLerpToTargetBehaviour template = new ResponsiveLerpToTargetBehaviour();

        public double startTime;
        public double endTime;

        [HideInInspector]
        public AppSettings appSettings;

        [HideInInspector]
        public TrackAsset parentTrack;

        [HideInInspector]
        public GameObject directorObject;
        
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
            // These are here for reference and don't do anything. These should also be implemented
            // in your child class so that the instance of your behaviour has all the necessary variables.
            template.startTime = startTime;
            template.endTime = endTime;
            template.appSettings = appSettings;
            template.parentTrack = parentTrack;
            template.clipAsset = this;
            template.directorObject = directorObject;

            var playable = ScriptPlayable<ResponsiveLerpToTargetBehaviour>.Create(graph);
            return playable;
        }
    }
}