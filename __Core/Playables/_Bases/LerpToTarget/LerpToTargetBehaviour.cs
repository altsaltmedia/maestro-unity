/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt
{
    public class LerpToTargetBehaviour : PlayableBehaviour
    {

        public EasingFunction.Ease ease = EasingFunction.Ease.EaseInOutQuad;

        [HideInInspector]
        public double startTime;

        [HideInInspector]
        public double endTime;

        [HideInInspector]
        public EasingFunction.Function easingFunction;

        [HideInInspector]
        public TrackAsset parentTrack;

        [HideInInspector]
        public PlayableAsset clipAsset;

        [HideInInspector]
        public GameObject directorObject;

        [HideInInspector]
        public SyncTimelineToSequence directorUpdater;

        public bool disableReset;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            easingFunction = EasingFunction.GetEasingFunction(ease);
        }
    }   
}