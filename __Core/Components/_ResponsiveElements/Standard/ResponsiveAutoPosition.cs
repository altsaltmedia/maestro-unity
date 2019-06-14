using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    public class ResponsiveAutoPosition : ResponsiveRectTransform
    {
        [SerializeField]
        List<float> multiplier = new List<float>();

#if UNITY_EDITOR
        void OnEnable()
        {
            UpdateBreakpointDependencies();
        }

        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();
            if (multiplier.Count == 0) {
                multiplier.Add(1f);
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
            double newDimension = (Math.Pow(0.561993755433366d, ((double)screenHeight.Value / (double)screenWidth.Value))) * 10.03014554127636d;

            rectTransform.anchoredPosition = new Vector2((float)newDimension * multiplier[activeIndex], rectTransform.anchoredPosition.y);
        }

    }
}