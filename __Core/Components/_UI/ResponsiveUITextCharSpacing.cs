using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class ResponsiveUITextCharSpacing : ResponsiveUIText, IResponsiveSaveable
    {
        public List<float> breakpointCharSpacing = new List<float>();

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

            Utils.ExpandList(breakpointCharSpacing, breakpointIndex);
            breakpointCharSpacing[breakpointIndex] = textMeshProUGUI.characterSpacing;

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
            textMeshProUGUI.characterSpacing = breakpointCharSpacing[activeIndex];
        }
    }
}