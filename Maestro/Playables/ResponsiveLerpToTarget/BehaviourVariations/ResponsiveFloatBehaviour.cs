using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ResponsiveFloatBehaviour : ResponsiveLerpToTargetBehaviour
    {
        // initialPosition and targetPosition don't refresh in inspector for
        // some reason, but they work as intended
        [HideInInspector]
        public float initialValue;
        [HideInInspector]
        public float targetValue;

        public List<float> breakpointInitialValue = new List<float>();
        public List<float> breakpointTargetValue = new List<float>();

#if UNITY_EDITOR
        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();

            if (HasBreakpoints == true) {

                int initialCount = breakpointInitialValue.Count;
                Utils.ExpandList(breakpointInitialValue, aspectRatioBreakpoints.Count);
                for (int i = initialCount; i < breakpointInitialValue.Count; i++) {
                    breakpointInitialValue[i] = breakpointInitialValue[initialCount - 1];
                }

                int targetCount = breakpointTargetValue.Count;
                Utils.ExpandList(breakpointTargetValue, aspectRatioBreakpoints.Count);
                for (int i = targetCount; i < breakpointTargetValue.Count; i++) {
                    breakpointTargetValue[i] = breakpointTargetValue[initialCount - 1];
                }

            }
        }

        public float GetInitialValueAtBreakpoint(float targetAspectRatio)
        {
            if (AspectRatioBreakpoints.Count > 0) {
                int breakpointIndex = Utils.GetValueIndexInList(targetAspectRatio, AspectRatioBreakpoints);
                return breakpointInitialValue[breakpointIndex];
            } else {
                return breakpointInitialValue[0];
            }
        }

        public List<float> SaveNewInitialValue(float targetValue)
        {
            if (HasBreakpoints == true) {
                int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, AspectRatioBreakpoints);
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
            if (AspectRatioBreakpoints.Count > 0) {
                int breakpointIndex = Utils.GetValueIndexInList(targetAspectRatio, AspectRatioBreakpoints);
                return breakpointTargetValue[breakpointIndex];
            } else {
                return breakpointTargetValue[0];
            }
        }

        public List<float> SaveNewTargetValue(float targetValue)
        {
            if (HasBreakpoints == true) {
                int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, AspectRatioBreakpoints);
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
