using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class ResponsiveTextSize : ResponsiveText, IResponsiveSaveable, IResponsiveWithBreakpoints
    {      
        public List<float> breakpointTextSize = new List<float>();

#if UNITY_EDITOR
        [HorizontalGroup("Split", 0.5f)]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [InfoBox("Saves the position at the current breakpoint index.")]
        public void SaveValue()
        {
            if (aspectRatioBreakpoints.Count < 1) {
                Debug.LogError("You must specify at least one breakpoint to save element values.", this);
                return;
            }

            int breakpointIndex = Utils.GetValueIndexInList(aspectRatio.Value, aspectRatioBreakpoints);

            Utils.ExpandList(breakpointTextSize, breakpointIndex);
            breakpointTextSize[breakpointIndex] = textMeshPro.fontSize;
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
            if (activeIndex >= breakpointTextSize.Count) {
                LogBreakpointMessage();
                return;
            }
#endif
            textMeshPro.fontSize = breakpointTextSize[activeIndex];
        }
    }
}