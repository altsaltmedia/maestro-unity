using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    [Serializable]
    public class ResponsiveVector3Behaviour : ResponsiveLerpToTargetBehaviour
    {
        // initialPosition and targetPosition don't refresh in inspector for
        // some reason, but they work as intended
        [HideInInspector]
        public Vector3 initialValue = new Vector3(0, 0, 0);
        [HideInInspector]
        public Vector3 targetValue = new Vector3(0, 0, 0);

        public List<Vector3> breakpointInitialValue = new List<Vector3>();
        public List<Vector3> breakpointTargetValue = new List<Vector3>();

#if UNITY_EDITOR
        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();

            if (HasBreakpoints == true) {

                int initialCount = breakpointInitialValue.Count;
                Utils.ExpandList(breakpointInitialValue, aspectRatioBreakpoints.Count);
                for(int i = initialCount; i<breakpointInitialValue.Count; i++) {
                    breakpointInitialValue[i] = breakpointInitialValue[initialCount - 1];
                }

                int targetCount = breakpointTargetValue.Count;
                Utils.ExpandList(breakpointTargetValue, aspectRatioBreakpoints.Count);
                for (int i = targetCount; i < breakpointTargetValue.Count; i++) {
                    breakpointTargetValue[i] = breakpointTargetValue[initialCount - 1];
                }

            }
        }

        public Vector3 GetInitialValueAtBreakpoint(float targetAspectRatio)
        {
            if(AspectRatioBreakpoints.Count > 0) {
                int breakpointIndex = Utils.GetValueIndexInList(targetAspectRatio, AspectRatioBreakpoints);
                return breakpointInitialValue[breakpointIndex];
            } else {
                return breakpointInitialValue[0];
            }
        }

        public List<Vector3> SaveNewInitialValue(Vector3 targetValue)
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


        public Vector3 GetTargetValueAtBreakpoint(float targetAspectRatio)
        {
            if (AspectRatioBreakpoints.Count > 0) {
                int breakpointIndex = Utils.GetValueIndexInList(targetAspectRatio, AspectRatioBreakpoints);
                return breakpointTargetValue[breakpointIndex];
            } else {
                return breakpointTargetValue[0];
            }
        }

        public List<Vector3> SaveNewTargetValue(Vector3 targetValue)
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
