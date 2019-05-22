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
        List<DimensionType> baseDimensionTypes = new List<DimensionType>();

        private ValueDropdownList<DimensionType> dimensionValues = new ValueDropdownList<DimensionType>(){
            {"Vertical", DimensionType.Vertical },
            {"Horizontal", DimensionType.Horizontal }
        };

        [ValueDropdown("aspectRatioValues")]
        [SerializeField]
        List<AspectRatioType> objectAspectRatios = new List <AspectRatioType>();

        private ValueDropdownList<AspectRatioType> aspectRatioValues = new ValueDropdownList<AspectRatioType>(){
            {"16 x 9", AspectRatioType.x16x9 },
            {"9 x 16", AspectRatioType.x9x16 },
            {"4 x 3", AspectRatioType.x4x3 }
        };

        [SerializeField]
        [Range(0, 3)]
        List<float> dimensionPercentages = new List<float>();

        [SerializeField]
        List<float> maxDimensionValues = new List<float>();

#if UNITY_EDITOR
        void OnEnable()
        {
            UpdateBreakpointDependencies();
        }

        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();
            if (baseDimensionTypes.Count == 0) {
                baseDimensionTypes.Add(DimensionType.Vertical);
            }
            if (objectAspectRatios.Count == 0) {
                objectAspectRatios.Add(AspectRatioType.x16x9);
            }
            if (dimensionPercentages.Count == 0) {
                dimensionPercentages.Add(1f);
            }
            if(maxDimensionValues.Count == 0) {
                maxDimensionValues.Add(0);
            }

            if(baseDimensionTypes.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(baseDimensionTypes, aspectRatioBreakpoints.Count);
            }
            if(objectAspectRatios.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(objectAspectRatios, aspectRatioBreakpoints.Count);
            }
            if (dimensionPercentages.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(dimensionPercentages, aspectRatioBreakpoints.Count);
            }
            if (maxDimensionValues.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(maxDimensionValues, aspectRatioBreakpoints.Count);
            }
        }

        void OnValidate()
        {
            UpdateBreakpointDependencies();
            ExecuteResponsiveAction();
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
            double baseDimension = (Math.Pow(0.561993755433366d, ((double)screenHeight.Value / (double)screenWidth.Value))) * 10.03014554127636d;
            baseDimension *= dimensionPercentages[activeIndex];
            if (maxDimensionValues[activeIndex] > 0 && baseDimension > maxDimensionValues[activeIndex]) {
                baseDimension = maxDimensionValues[activeIndex];
            }

            double dependentDimension;
            if (baseDimensionTypes[activeIndex] == DimensionType.Vertical) {
                dependentDimension = GetDependentDimension(objectAspectRatios[activeIndex], baseDimension, RatioType.Denominator);
                rectTransform.localScale = new Vector2((float)dependentDimension, (float)baseDimension);
            } else {
                dependentDimension = GetDependentDimension(objectAspectRatios[activeIndex], baseDimension, RatioType.Numerator);
                rectTransform.localScale = new Vector2((float)baseDimension, (float)dependentDimension);
            }
        }

        double GetDependentDimension(AspectRatioType referenceAspectRatio, double referenceVal, RatioType ratioType)
        {
            if (referenceAspectRatio == AspectRatioType.x16x9) {

                if (ratioType == RatioType.Numerator) {
                    return (referenceVal * 9.0d / 16.0d);
                } else {
                    return (referenceVal * 16.0d / 9.0d);
                }

            } else if (referenceAspectRatio == AspectRatioType.x9x16) {

                if (ratioType == RatioType.Numerator) {
                    return (referenceVal * 16.0d / 9.0d);
                } else {
                    return (referenceVal * 9.0d / 16.0d);
                }

            } else {

                if (ratioType == RatioType.Numerator) {
                    return (referenceVal * 3.0d / 4.0d);
                } else {
                    return (referenceVal * 4.0d / 3.0d);
                }
            }
        }

    }

}