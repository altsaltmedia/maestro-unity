using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
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
        [HorizontalGroup("Split", 0.5f)]
        [InfoBox("Saves the position at the current breakpoint index.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SaveValue()
        {
            if (aspectRatioBreakpoints.Count < 1) {
                Debug.LogError("You must specify at least one breakpoint to save element values.", this);
                return;
            }

            int breakpointIndex = Utils.GetValueIndexInList(aspectRatio.Value, aspectRatioBreakpoints);

            Utils.ExpandList(breakpointAnchorMax, breakpointIndex);
            breakpointAnchorMax[breakpointIndex] = rectTransform.anchorMax;

            Utils.ExpandList(breakpointAnchorMin, breakpointIndex);
            breakpointAnchorMin[breakpointIndex] = rectTransform.anchorMin;

            Utils.ExpandList(breakpointSizeDelta, breakpointIndex);
            breakpointSizeDelta[breakpointIndex] = rectTransform.sizeDelta;
        }
#endif

        protected override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            int breakpointIndex = Utils.GetValueIndexInList(aspectRatio.Value, aspectRatioBreakpoints);
            SetValue(breakpointIndex);
        }

        public void SetValue(int activeIndex)
        {
            rectTransform.sizeDelta = breakpointSizeDelta[activeIndex];
        }
    }

}