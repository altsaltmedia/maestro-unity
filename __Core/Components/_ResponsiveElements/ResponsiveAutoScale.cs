using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class ResponsiveAutoScale : ResponsiveRectTransform
    {

        [ValueDropdown("dimensionValues")]
        [SerializeField]
        DimensionType baseDimensionType;

        private ValueDropdownList<DimensionType> dimensionValues = new ValueDropdownList<DimensionType>(){
            {"Vertical", DimensionType.Vertical },
            {"Horizontal", DimensionType.Horizontal }
        };

        [ValueDropdown("aspectRatioValues")]
        [SerializeField]
        AspectRatioType objectAspectRatio;

        private ValueDropdownList<AspectRatioType> aspectRatioValues = new ValueDropdownList<AspectRatioType>(){
            {"16 x 9", AspectRatioType.x16x9 },
            {"9 x 16", AspectRatioType.x9x16 },
            {"4 x 3", AspectRatioType.x4x3 }
        };

        [SerializeField]
        [Range(0, 3)]
        float dimensionPercentage = 1f;

        [SerializeField]
        float maxDimensionValue;

        protected override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            // Custom equation of an exponential function - equation is in the form y = a^x * b
            // It is derived by taking two (X,Y) coordinates along the line, creating two equations
            // in the form above, then dividing one equation by the other to solve for a and b.
            double baseDimension = (Math.Pow(0.561993755433366d, ((double)screenHeight.Value / (double)screenWidth.Value))) * 10.03014554127636d;
            baseDimension *= dimensionPercentage;
            if (maxDimensionValue > 0 && baseDimension > maxDimensionValue) {
                baseDimension = maxDimensionValue;
            }

            double dependentDimension = 0d;
            if (baseDimensionType == DimensionType.Vertical) {
                dependentDimension = GetDependentDimension(objectAspectRatio, baseDimension, RatioType.Denominator);
                rectTransform.localScale = new Vector2((float)dependentDimension, (float)baseDimension);
            }
            else {
                dependentDimension = GetDependentDimension(objectAspectRatio, baseDimension, RatioType.Numerator);
                rectTransform.localScale = new Vector2((float)baseDimension, (float)dependentDimension);
            }
        }

        double GetDependentDimension(AspectRatioType referenceAspectRatio, double referenceVal, RatioType ratioType)
        {

            if (referenceAspectRatio == AspectRatioType.x16x9) {

                if (ratioType == RatioType.Numerator) {
                    return (referenceVal * 9.0d / 16.0d);
                }
                else {
                    return (referenceVal * 16.0d / 9.0d);
                }

            }
            else if (referenceAspectRatio == AspectRatioType.x9x16) {

                if (ratioType == RatioType.Numerator) {
                    return (referenceVal * 16.0d / 9.0d);
                }
                else {
                    return (referenceVal * 9.0d / 16.0d);
                }

            }
            else {

                if (ratioType == RatioType.Numerator) {
                    return (referenceVal * 3.0d / 4.0d);
                }
                else {
                    return (referenceVal * 4.0d / 3.0d);
                }
            }
        }

        void OnValidate()
        {
            ExecuteResponsiveAction();
        }

    }

}