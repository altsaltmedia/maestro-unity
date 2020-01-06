/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [Serializable]
    public abstract class LerpToTargetBehaviour : PlayableBehaviour
    {
        // App Settings is populated dynamically via LerpToTargetTrack at runtime
        [Required]
        [ReadOnly]
        private AppSettings _appSettings;

        public AppSettings appSettings
        {
            get => _appSettings;
            set => _appSettings = value;
        }
        
        [SerializeField]
        [FormerlySerializedAs("ease")]
        private EasingFunction.Ease _ease = EasingFunction.Ease.EaseInOutQuad;

        public EasingFunction.Ease ease
        {
            get => _ease;
            set => _ease = value;
        }

        [HideInInspector]
        private double _startTime;

        public double startTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        [HideInInspector]
        private double _endTime;

        public double endTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        [HideInInspector]
        private EasingFunction.Function _easingFunction;

        public EasingFunction.Function easingFunction
        {
            get => _easingFunction;
            set => _easingFunction = value;
        }

        [HideInInspector]
        private TrackAsset _parentTrack;

        public TrackAsset parentTrack
        {
            get => _parentTrack;
            set => _parentTrack = value;
        }

        [HideInInspector]
        private PlayableAsset _clipAsset;

        public PlayableAsset clipAsset
        {
            get => _clipAsset;
            set => _clipAsset = value;
        }

        [HideInInspector]
        private GameObject _directorObject;

        public GameObject directorObject
        {
            get => _directorObject;
            set => _directorObject = value;
        }

        public bool disableReset;

#if UNITY_EDITOR
        public virtual object SetInitialValueToTarget()
        {
            return null;
        }
#endif

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            easingFunction = EasingFunction.GetEasingFunction(ease);
        }
    }   
}