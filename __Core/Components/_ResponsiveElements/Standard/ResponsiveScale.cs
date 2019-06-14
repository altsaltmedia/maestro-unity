using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class ResponsiveScale : ResponsiveRectTransform, IResponsiveSaveable
    {

        public List<Vector2> breakpointLocalScale = new List<Vector2>();

#if UNITY_EDITOR
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [InfoBox("Saves the position at the current breakpoint index.")]
        public void SaveValue()
        {
            if (aspectRatioBreakpoints.Count < 1) {
                Debug.LogError("You must specify at least one breakpoint to save element values.", this);
                return;
            }

            int targetBreakpointIndex = Utils.GetValueIndexInList(aspectRatio.Value, aspectRatioBreakpoints);

            Utils.ExpandList(breakpointLocalScale, targetBreakpointIndex);
            breakpointLocalScale[targetBreakpointIndex] = rectTransform.localScale;
        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            SetValue(breakpointIndex);
        }

        public void SetValue(int activeIndex)
        {
            rectTransform.localScale = breakpointLocalScale[activeIndex];
        }

    }
    
}