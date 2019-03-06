using System;
using UnityEngine;

namespace AltSalt
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class ResponsiveAutoScaleAdjust : ResponsiveRectTransform
    {
        public float widthMultiplier = 1f;

        protected override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            // Custom equation of an exponential function - equation is in the form y = a^x * b
            // It is derived by taking two (X,Y) coordinates along the line, creating two equations
            // in the form above, then dividing one equation by the other to solve for a and b.
            double width = (Math.Pow(0.561993755433366d, ((double)screenHeight.Value / (double)screenWidth.Value))) * 10.03014554127636d;

            rectTransform.localScale = new Vector2((float)width * widthMultiplier, rectTransform.sizeDelta.y);
        }
    }
}