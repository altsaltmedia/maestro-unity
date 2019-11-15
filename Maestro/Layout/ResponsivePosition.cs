using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace AltSalt.Maestro.Layout
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
            GetBreakpointIndex();

            if(hasBreakpoints == true) {
                if(aspectRatioBreakpoints.Count < 1) {
                    Debug.LogWarning("You must specify at least one breakpoint to save element values.", this);
                    return;
                }
            }

            Utils.ExpandList(breakpointAnchorMax, breakpointIndex);
            breakpointAnchorMax[breakpointIndex] = rectTransform.anchorMax;

            Utils.ExpandList(breakpointAnchorMin, breakpointIndex);
            breakpointAnchorMin[breakpointIndex] = rectTransform.anchorMin;

            Utils.ExpandList(breakpointAnchoredPosition, breakpointIndex);
            breakpointAnchoredPosition[breakpointIndex] = rectTransform.anchoredPosition3D;
        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            SetValue(breakpointIndex);
        }

        public void SetValue(int activeIndex)
        {
            if(hasBreakpoints == true) {

                if (activeIndex >= breakpointAnchorMax.Count ||
                    activeIndex >= breakpointAnchorMin.Count ||
                    activeIndex >= breakpointAnchoredPosition.Count) {
                    LogBreakpointError();
                    return;
                }
            } else {
                if (breakpointAnchorMax.Count < 1 ||
                    breakpointAnchorMin.Count < 1 ||
                    breakpointAnchoredPosition.Count < 1) {
                    LogBreakpointError();
                    return;
                }
            }
#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(rectTransform, "set responsive position");
#endif

            rectTransform.anchorMax = breakpointAnchorMax[activeIndex];
            rectTransform.anchorMin = breakpointAnchorMin[activeIndex];
            rectTransform.anchoredPosition3D = breakpointAnchoredPosition[activeIndex];
        }
    }   
}