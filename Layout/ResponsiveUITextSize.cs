using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveUITextSize : ResponsiveUIText, IResponsiveSaveable
    {
        public List<float> breakpointTextSize = new List<float>();

#if UNITY_EDITOR
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [InfoBox("Saves the position at the current breakpoint index.")]
        public void SaveValue()
        {
            if (_aspectRatioBreakpoints.Count < 1) {
                Debug.LogError("You must specify at least one breakpoint to save element values.", this);
                return;
            }

            int targetBreakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, _aspectRatioBreakpoints);

            Utils.ExpandList(breakpointTextSize, targetBreakpointIndex);
            breakpointTextSize[targetBreakpointIndex] = textMeshProUGUI.fontSize;

        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            SetValue(breakpointIndex);
        }

        public void SetValue(int activeIndex)
        {
            textMeshProUGUI.fontSize = breakpointTextSize[activeIndex];
        }
    }
}