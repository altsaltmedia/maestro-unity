using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class ResponsiveRectTransformPosBehaviour : ResponsiveLerpToTargetBehaviour, IResponsiveSaveable
    {
        // initialPosition and targetPosition don't refresh in inspector for
        // some reason, but they work as intended
        [HideInInspector]
        public Vector3 initialPosition = new Vector3(0, 0, 0);
        [HideInInspector]
        public Vector3 targetPosition = new Vector3(0, 0, 0);

        public List<Vector3> breakpointInitialPosition = new List<Vector3>();
        public List<Vector3> breakpointTargetPosition = new List<Vector3>();

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
#if UNITY_EDITOR
            if (aspectRatioBreakpoints.Count == 0) {
                SetValue(0);
            } else {
                int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, aspectRatioBreakpoints);
                SetValue(breakpointIndex);
            }
#endif
        }

#if UNITY_EDITOR

        protected override void PopulateBreakpointDependencies()
        {
            base.PopulateBreakpointDependencies();

            if(breakpointInitialPosition.Count < 1) {
                breakpointInitialPosition.Add(new Vector3(0, 0, 0));
            }

            if (breakpointTargetPosition.Count < 1) {
                breakpointTargetPosition.Add(new Vector3(0, 0, 0));
            }
        }

        [InfoBox("Creates placeholder values based on number of breakpoints.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SaveValue()
        {
            if (aspectRatioBreakpoints.Count < 1) {
                Debug.LogWarning("You must specify at least one breakpoint to save element values.");
                return;
            }

            int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, aspectRatioBreakpoints);

            Utils.ExpandList(breakpointInitialPosition, breakpointIndex);
            breakpointInitialPosition[breakpointIndex] = new Vector3(0,0,0);

            Utils.ExpandList(breakpointTargetPosition, breakpointIndex);
            breakpointTargetPosition[breakpointIndex] = new Vector3(0,0,0);
        }
#endif
        public void SetValue(int activeIndex)
        {
#if UNITY_EDITOR
            if(activeIndex != 0 &&
               (activeIndex >= breakpointInitialPosition.Count ||
               activeIndex >= breakpointTargetPosition.Count)) {
                LogBreakpointWarning();
                return;
            }
#endif
            initialPosition = breakpointInitialPosition[activeIndex];
            targetPosition = breakpointTargetPosition[activeIndex];
        }

    }
}