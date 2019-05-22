using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class ResponsivePosition : ResponsiveRectTransform, IResponsiveSaveable
    {
        public List<Vector2> breakpointAnchorMax = new List<Vector2>();
        public List<Vector2> breakpointAnchorMin = new List<Vector2>();
        public List<Vector3> breakpointAnchoredPosition = new List<Vector3>();

#if UNITY_EDITOR
        [InfoBox("Saves the position at the current breakpoint index.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SaveValue()
        {
            if(aspectRatioBreakpoints.Count < 1) {
                Debug.LogWarning("You must specify at least one breakpoint to save element values.", this);
                return;
            }

            int targetBreakpointIndex = Utils.GetValueIndexInList(aspectRatio.Value, aspectRatioBreakpoints);

            Utils.ExpandList(breakpointAnchorMax, targetBreakpointIndex);
            breakpointAnchorMax[targetBreakpointIndex] = rectTransform.anchorMax;

            Utils.ExpandList(breakpointAnchorMin, targetBreakpointIndex);
            breakpointAnchorMin[targetBreakpointIndex] = rectTransform.anchorMin;

            Utils.ExpandList(breakpointAnchoredPosition, targetBreakpointIndex);
            breakpointAnchoredPosition[targetBreakpointIndex] = rectTransform.anchoredPosition3D;
        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            SetValue(breakpointIndex);
        }

        public void SetValue(int activeIndex)
        {
#if UNITY_EDITOR
            if (activeIndex >= breakpointAnchorMax.Count ||
                activeIndex >= breakpointAnchorMin.Count ||
                activeIndex >= breakpointAnchoredPosition.Count) {
                LogBreakpointError();
                return;
            }
#endif
            rectTransform.anchorMax = breakpointAnchorMax[activeIndex];
            rectTransform.anchorMin = breakpointAnchorMin[activeIndex];
            rectTransform.anchoredPosition3D = breakpointAnchoredPosition[activeIndex];
        }
    }   
}