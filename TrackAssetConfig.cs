using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    public class TrackAssetConfig : MonoBehaviour
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettings _appSettings;

        public AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
            set => _appSettings = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private BoolReference _isReversing;

        public BoolVariable isReversingVariable
        {
            get => _isReversing.GetVariable(this.gameObject);
            private set => _isReversing.SetVariable(value);
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private BoolReference _scrubberActive;

        public BoolVariable scrubberActiveVariable
        {
            get => _scrubberActive.GetVariable(this.gameObject);
            private set => _scrubberActive.SetVariable(value);
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _timelineCurrentTime;

        public FloatVariable timelineCurrentTime
        {
            get => _timelineCurrentTime.variable;
            private set => _timelineCurrentTime.variable = value;
        }

        [SerializeField]
        [Required]
        private SimpleEvent _onGraphStart;

        public SimpleEvent onGraphStart
        {
            get => _onGraphStart;
            private set => _onGraphStart = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _frameStepValue;

        public FloatVariable frameStepValue
        {
            get => _frameStepValue.variable;
            private set => _frameStepValue.variable = value;
        }
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _swipeModifierOutput;

        public FloatVariable swipeModifierOutput
        {
            get => _swipeModifierOutput.variable;
            private set => _swipeModifierOutput.variable = value;
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
                if (appSettings == null) {
                    appSettings = Utils.GetAppSettings();
                }

                if (isReversingVariable == null) {
                    isReversingVariable = Utils.GetBoolVariable(nameof(VarDependencies.IsReversing));
                }

                if (scrubberActiveVariable == null) {
                    scrubberActiveVariable = Utils.GetBoolVariable(nameof(VarDependencies.ScrubberActive));
                }

                if (timelineCurrentTime == null) {
                    timelineCurrentTime = Utils.GetFloatVariable(nameof(VarDependencies.TimelineCurrentTime));
                }

                if (onGraphStart == null) {
                    onGraphStart = Utils.GetSimpleEvent(nameof(VarDependencies.OnGraphStart));
                }

                if (frameStepValue == null) {
                    frameStepValue = Utils.GetFloatVariable(nameof(VarDependencies.FrameStepValue));
                }

                if (swipeModifierOutput == null) {
                    swipeModifierOutput = Utils.GetFloatVariable(nameof(VarDependencies.SwipeModifierOutput));
                }
            #endif
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