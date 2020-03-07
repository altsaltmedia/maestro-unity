using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveWidthHeight : ResponsiveRectTransform, IResponsiveSaveable
    {
        [InfoBox("Sets width and height to manually configured values based on breakpoint.")]
        public List<Vector2> breakpointSizeDelta = new List<Vector2>();
        
#if UNITY_EDITOR
        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateBreakpointDependencies();
        }
        
        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();
            if (breakpointSizeDelta.Count == 0) {
                breakpointSizeDelta.Add(new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y));
            }
            
            if (breakpointSizeDelta.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(breakpointSizeDelta, aspectRatioBreakpoints.Count);
            }
        }
        
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
            if(hasBreakpoints == true) {
                SetValue(breakpointIndex);
            } else {
                SetValue(0);
            }
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