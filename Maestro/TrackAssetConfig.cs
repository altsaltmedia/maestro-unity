using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    public class TrackAssetConfig : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private AppSettings _appSettings;

        public AppSettings appSettings
        {
            get =>  _appSettings;
            private set => _appSettings = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private BoolReference _isReversing;

        public BoolVariable isReversing
        {
            get => _isReversing.Variable;
            private set => _isReversing.Variable = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private BoolReference _scrubberActive;

        public BoolVariable scrubberActive
        {
            get => _scrubberActive.Variable;
            private set => _scrubberActive.Variable = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _timelineCurrentTime;

        public FloatVariable timelineCurrentTime
        {
            get => _timelineCurrentTime.Variable;
            private set => _timelineCurrentTime.Variable = value;
        }

        [SerializeField]
        [Required]
        private SimpleEvent _onGraphStart;

        public SimpleEvent onGraphStart
        {
            get => _onGraphStart;
            private set => _onGraphStart = value;
        }

        private void OnEnable()
        {
            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }

            if (isReversing == null) {
                isReversing = Utils.GetBoolVariable(nameof(VarDependencies.IsReversing));
            }

            if (scrubberActive == null) {
                scrubberActive = Utils.GetBoolVariable(nameof(VarDependencies.ScrubberActive));
            }

            if (timelineCurrentTime == null) {
                timelineCurrentTime = Utils.GetFloatVariable(nameof(VarDependencies.TimelineCurrentTime));
            }

            if (onGraphStart == null) {
                onGraphStart = Utils.GetSimpleEvent(nameof(VarDependencies.OnGraphStart));
            }
            
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}