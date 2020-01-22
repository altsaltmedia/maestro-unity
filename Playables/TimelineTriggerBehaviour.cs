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

        public bool isReversing => trackAssetConfig.isReversing;

        [FormerlySerializedAs("disableOnReverse")]
        [SerializeField]
        private bool _disableOnReverse;

        public bool disableOnReverse
        {
            get => _disableOnReverse;
            set => _disableOnReverse = value;
        }
        
        [HideInInspector]
        private TrackAssetConfig _trackAssetConfig;

        public TrackAssetConfig trackAssetConfig
        {
            get => _trackAssetConfig;
            set => _trackAssetConfig = value;
        }

    }
}