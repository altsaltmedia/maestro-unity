using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class ResponsiveAutoScaleStretch : ResponsiveRectTransform
    {
        [SerializeField]
        List<float> widthMultipliers = new List<float>();

        [SerializeField]
        List<float> heightMultipliers = new List<float>();


#if UNITY_EDITOR
        void OnEnable()
        {
            UpdateBreakpointDependencies();
        }

        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();
            if (widthMultipliers.Count == 0) {
                widthMultipliers.Add(1f);
            }
            if (heightMultipliers.Count == 0) {
                heightMultipliers.Add(1f);
            }
            if (widthMultipliers.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(widthMultipliers, aspectRatioBreakpoints.Count);
            }
        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            SetValue(breakpointIndex);
        }

        void SetValue(int activeIndex)
        {
            // Custom equation of an exponential function - equation is in the form y = a^x * b
            // It is derived by taking two (X,Y) coordinates along the line, creating two equations
            // in the form above, then dividing one equation by the other to solve for a and b.
            double width = (Math.Pow(0.561993755433366d, ((double)screenHeight.Value / (double)screenWidth.Value))) * 10.03014554127636d;
            float height = Utils.pageHeight * heightMultipliers[activeIndex];

            rectTransform.localScale = new Vector2((float)width * widthMultipliers[activeIndex], height);
        }
    }
}