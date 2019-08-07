using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class ResponsiveRectTransformScaleBehaviour : ResponsiveLerpToTargetBehaviour, IResponsiveSaveable
    {
        // initialScale and targetScale don't refresh in inspector for
        // some reason, but they work as intended
        [HideInInspector]
        public Vector3 initialScale = new Vector3(0, 0, 0);
        [HideInInspector]
        public Vector3 targetScale = new Vector3(0, 0, 0);

        public List<Vector3> breakpointinitialScale = new List<Vector3>();
        public List<Vector3> breakpointtargetScale = new List<Vector3>();

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
#if UNITY_EDITOR
            if (aspectRatioBreakpoints.Count < 1) {
                return;
            }
#endif
            int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, aspectRatioBreakpoints);
            SetValue(breakpointIndex);
        }

#if UNITY_EDITOR
        
        [InfoBox("Creates placeholder values based on number of breakpoints.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SaveValue()
        {
            if (aspectRatioBreakpoints.Count < 1) {
                Debug.LogWarning("You must specify at least one breakpoint to save element values.");
                return;
            }

            int breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, aspectRatioBreakpoints);

            Utils.ExpandList(breakpointinitialScale, breakpointIndex);
            breakpointinitialScale[breakpointIndex] = new Vector3(0,0,0);

            Utils.ExpandList(breakpointtargetScale, breakpointIndex);
            breakpointtargetScale[breakpointIndex] = new Vector3(0,0,0);
        }
#endif

        public void SetValue(int activeIndex)
        {
#if UNITY_EDITOR
            if(activeIndex >= breakpointinitialScale.Count ||
               activeIndex >= breakpointtargetScale.Count) {
                return;
            }
#endif
            initialScale = breakpointinitialScale[activeIndex];
            targetScale = breakpointtargetScale[activeIndex];
        }

    }
}