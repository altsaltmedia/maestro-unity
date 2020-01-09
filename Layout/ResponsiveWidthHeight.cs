using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveWidthHeight : ResponsiveRectTransform, IResponsiveSaveable
    {
        
        public List<Vector2> breakpointSizeDelta = new List<Vector2>();
        
#if UNITY_EDITOR
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [InfoBox("Saves the position at the current breakpoint index.")]
        public void SaveValue()
        {
            if (aspectRatioBreakpoints.Count < 1) {
                Debug.LogWarning("You must specify at least one breakpoint to save element values.", this);
                return;
            }

            int targetBreakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio, aspectRatioBreakpoints);

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
#if UNITY_EDITOR
            if (activeIndex >= breakpointSizeDelta.Count) {
                LogBreakpointError();
                return;
            }
#endif
            rectTransform.sizeDelta = breakpointSizeDelta[activeIndex];
        }

    }
    
}