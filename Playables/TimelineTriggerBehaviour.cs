using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public abstract class TimelineTriggerBehaviour : PlayableBehaviour {

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
        private bool _triggered = false;

        public bool triggered
        {
            get => _triggered;
            set => _triggered = value;
        }

        [NonSerialized]
        public BoolVariable _isReversingVariable;

        public bool isReversing => _isReversingVariable.value;

        [SerializeField]
        private bool _executeWhileAppUtilsRequested;

        public bool executeWhileAppUtilsRequested => _executeWhileAppUtilsRequested;
        
        public abstract bool disableOnReverse { get; }

        public abstract bool disableOnForward { get; }
        
        public abstract bool executeWhileLoadingBookmarks { get; }
        
        // Not all triggers should be activated if we're past the clip
        // thresholds. While in general, we DO want to execute any events that
        // affect the game state (loading scenes, changing variables, etc.)
        // no matter what, other triggers, such as those used to
        // play audio clips, we only want to execute within the confines
        // of the clip's duration.
        public abstract bool forceActivateOnForward { get; }
        
        public abstract bool forceActivateOnReverse { get; }
        
        [HideInInspector]
        private TimelineInstanceConfig _timelineInstanceConfig;

        public TimelineInstanceConfig timelineInstanceConfig
        {
            get => _timelineInstanceConfig;
            set => _timelineInstanceConfig = value;
        }

    }
}