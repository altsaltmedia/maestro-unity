using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ResponsiveFloatBehaviour : ResponsiveLerpToTargetBehaviour
    {
        [ShowInInspector]
        private bool _editMode;

        private bool editMode
        {
            get => _editMode;
            set => _editMode = value;
        }
        
        // initialPosition and targetPosition don't refresh in inspector for
        // some reason, but they work as intended
        [ShowIf(nameof(editMode))]
        [ShowInInspector]
        private float _initialValue;

        public float initialValue
        {
            get => _initialValue;
            set => _initialValue = value;
        }

        [ShowIf(nameof(editMode))]
        [ShowInInspector]
        private float _targetValue;

        public float targetValue
        {
            get => _targetValue;
            set => _targetValue = value;
        }

        [FormerlySerializedAs("breakpointInitialValue")]
        [SerializeField]
        private List<float> _breakpointInitialValue = new List<float>();

        public List<float> breakpointInitialValue
        {
            get => _breakpointInitialValue;
            set => _breakpointInitialValue = value;
        }

        [FormerlySerializedAs("breakpointTargetValue")]
        [SerializeField]
        private List<float> _breakpointTargetValue = new List<float>();

        public List<float> breakpointTargetValue
        {
            get => _breakpointTargetValue;
            set => _breakpointTargetValue = value;
        }

#if UNITY_EDITOR
        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();

            if (hasBreakpoints == true) {

                int initialCount = breakpointInitialValue.Count;
                Utils.ExpandList(breakpointInitialValue, _aspectRatioBreakpoints.Count);
                for (int i = initialCount; i < breakpointInitialValue.Count; i++) {
                    breakpointInitialValue[i] = breakpointInitialValue[initialCount - 1];
                }

                int targetCount = breakpointTargetValue.Count;
                Utils.ExpandList(breakpointTargetValue, _aspectRatioBreakpoints.Count);
                for (int i = targetCount; i < breakpointTargetValue.Count; i++) {
                    breakpointTargetValue[i] = breakpointTargetValue[initialCount - 1];
                }

            }
        }

        public float GetInitialValueAtBreakpoint(float targetAspectRatio)
        {
            if (aspectRatioBreakpoints.Count > 0) {
                int breakpointIndex = Utils.GetValueIndexInList(targetAspectRatio, aspectRatioBreakpoints);
                return breakpointInitialValue[breakpointIndex];
            } else {
                return breakpointInitialValue[0];
            }
        }

        public List<float> SaveNewInitialValue(float targetValue)
        {
            if (hasBreakpoints == true) {
                int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, aspectRatioBreakpoints);
                breakpointInitialValue[breakpointIndex] = targetValue;
            } else {
                breakpointInitialValue[0] = targetValue;
            }
            UpdateBreakpointDependencies();
            CallExecuteLayoutUpdate(directorObject);
            return breakpointInitialValue;
        }


        public float GetTargetValueAtBreakpoint(float targetAspectRatio)
        {
            if (aspectRatioBreakpoints.Count > 0) {
                int breakpointIndex = Utils.GetValueIndexInList(targetAspectRatio, aspectRatioBreakpoints);
                return breakpointTargetValue[breakpointIndex];
            } else {
                return breakpointTargetValue[0];
            }
        }

        public List<float> SaveNewTargetValue(float targetValue)
        {
            if (hasBreakpoints == true) {
                int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, aspectRatioBreakpoints);
                breakpointTargetValue[breakpointIndex] = targetValue;
            } else {
                breakpointTargetValue[0] = targetValue;
            }
            UpdateBreakpointDependencies();
            CallExecuteLayoutUpdate(directorObject);
            return breakpointTargetValue;
        }
#endif

        protected override void SetValue(int activeIndex)
        {
#if UNITY_EDITOR
            if (activeIndex != 0 &&
               (activeIndex >= breakpointInitialValue.Count ||
                activeIndex >= breakpointTargetValue.Count)) {
                LogBreakpointWarning();
                return;
            }
#endif
            initialValue = breakpointInitialValue[activeIndex];
            targetValue = breakpointTargetValue[activeIndex];
        }
    }
}
