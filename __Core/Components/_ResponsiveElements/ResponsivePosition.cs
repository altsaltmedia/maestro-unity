using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class ResponsivePosition : ResponsiveRectTransform, IResponsiveSaveable, IResponsiveWithBreakpoints
    {
        
        public List<Vector2> breakpointAnchorMax = new List<Vector2>();
        public List<Vector2> breakpointAnchorMin = new List<Vector2>();
        public List<Vector3> breakpointAnchoredPosition = new List<Vector3>();

#if UNITY_EDITOR
        [HorizontalGroup("Split", 0.5f)]
        [InfoBox("Saves the position at the current breakpoint index.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SaveValue()
        {
            if(aspectRatioBreakpoints.Count < 1) {
                Debug.LogWarning("You must specify at least one breakpoint to save element values.", this);
                return;
            }

            int breakpointIndex = Utils.GetValueIndexInList(aspectRatio.Value, aspectRatioBreakpoints);

            Utils.ExpandList(breakpointAnchorMax, breakpointIndex);
            breakpointAnchorMax[breakpointIndex] = rectTransform.anchorMax;

            Utils.ExpandList(breakpointAnchorMin, breakpointIndex);
            breakpointAnchorMin[breakpointIndex] = rectTransform.anchorMin;

            Utils.ExpandList(breakpointAnchoredPosition, breakpointIndex);
            breakpointAnchoredPosition[breakpointIndex] = rectTransform.anchoredPosition3D;
        }

        public override void Reset()
        {
            base.Reset();
            hasBreakpoints = true;
        }

        public void LogBreakpointMessage()
        {
            Debug.Log("Please specify at least one breakpoint and two corresponding values on " + this.name, this);
        }
#endif

        protected override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
#if UNITY_EDITOR
            if (aspectRatioBreakpoints.Count < 1) {
                LogBreakpointMessage();
                return;
            }
#endif
            int breakpointIndex = Utils.GetValueIndexInList(aspectRatio.Value, aspectRatioBreakpoints);
            SetValue(breakpointIndex);
        }

        public void SetValue(int activeIndex)
        {
#if UNITY_EDITOR
            if (activeIndex >= breakpointAnchorMax.Count ||
                activeIndex >= breakpointAnchorMin.Count ||
                activeIndex >= breakpointAnchoredPosition.Count) {
                LogBreakpointMessage();
                return;
            }
#endif
            rectTransform.anchorMax = breakpointAnchorMax[activeIndex];
            rectTransform.anchorMin = breakpointAnchorMin[activeIndex];
            rectTransform.anchoredPosition3D = breakpointAnchoredPosition[activeIndex];
        }
    }   
}