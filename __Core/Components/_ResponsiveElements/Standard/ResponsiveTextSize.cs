﻿using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class ResponsiveTextSize : ResponsiveText, IResponsiveSaveable
    {      
        public List<float> breakpointTextSize = new List<float>();

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

            Utils.ExpandList(breakpointTextSize, targetBreakpointIndex);
            breakpointTextSize[targetBreakpointIndex] = textMeshPro.fontSize;
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
            if (activeIndex >= breakpointTextSize.Count) {
                LogBreakpointError();
                return;
            }
#endif
            textMeshPro.fontSize = breakpointTextSize[activeIndex];
        }
    }
}