using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class ResponsiveAutoUIHeight : ResponsiveRectTransform, IResponsiveSaveable
    {

        public List<Vector2> breakpointAnchorMax = new List<Vector2>();
        public List<Vector2> breakpointAnchorMin = new List<Vector2>();
        public List<Vector2> breakpointSizeDelta = new List<Vector2>();

#if UNITY_EDITOR
        
        [InfoBox("Saves the position at the current breakpoint index.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SaveValue()
        {
            if (_aspectRatioBreakpoints.Count < 1) {
                Debug.LogError("You must specify at least one breakpoint to save element values.", this);
                return;
            }

            int targetBreakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, _aspectRatioBreakpoints);

            Utils.ExpandList(breakpointAnchorMax, targetBreakpointIndex);
            breakpointAnchorMax[targetBreakpointIndex] = rectTransform.anchorMax;

            Utils.ExpandList(breakpointAnchorMin, targetBreakpointIndex);
            breakpointAnchorMin[targetBreakpointIndex] = rectTransform.anchorMin;

            Utils.ExpandList(breakpointSizeDelta, targetBreakpointIndex);
            breakpointSizeDelta[targetBreakpointIndex] = rectTransform.sizeDelta;
        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            SetValue(breakpointIndex);
        }

        public void SetValue(int activeIndex)
        {
            rectTransform.sizeDelta = breakpointSizeDelta[activeIndex];
        }
    }

}