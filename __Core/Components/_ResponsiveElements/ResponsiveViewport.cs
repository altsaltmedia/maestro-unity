using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class ResponsiveViewport : ResponsiveCamera, IResponsiveSaveable, IResponsiveWithBreakpoints
    {
        [SerializeField]
        public List<Rect> viewportValues = new List<Rect>();

#if UNITY_EDITOR
        [HorizontalGroup("Split", 0.5f)]
        [InfoBox("Saves the viewport value at the current breakpoint index.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SaveValue()
        {
            if (aspectRatioBreakpoints.Count < 1) {
                Debug.LogWarning("You must specify at least one breakpoint to save element values.", this);
                return;
            }

            int breakpointIndex = Utils.GetValueIndexInList(aspectRatio.Value, aspectRatioBreakpoints);

            Utils.ExpandList(viewportValues, breakpointIndex);
            viewportValues[breakpointIndex] = thisCamera.rect;
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
            if (activeIndex >= viewportValues.Count) {
                LogBreakpointMessage();
                return;
            }
#endif
            thisCamera.rect = viewportValues[activeIndex];
        }

    }   
}