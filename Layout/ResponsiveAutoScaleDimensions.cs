using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{

    [ExecuteInEditMode]
    public class ResponsiveAutoScaleDimensions : ResponsiveRectTransform
    {
        [ValueDropdown(nameof(dimensionValues))]
        [SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(UpdateDimensions))]
#endif
        List<DimensionType> baseDimensionTypes = new List<DimensionType>();

        private ValueDropdownList<DimensionType> dimensionValues = new ValueDropdownList<DimensionType>(){
            {"Vertical", DimensionType.Vertical },
            {"Horizontal", DimensionType.Horizontal }
        };

        [ValueDropdown(nameof(aspectRatioValues))]
        [SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(UpdateDimensions))]
#endif
        List<AspectRatioType> objectAspectRatios = new List <AspectRatioType>();

        private ValueDropdownList<AspectRatioType> aspectRatioValues = new ValueDropdownList<AspectRatioType>(){
            {"16 x 9", AspectRatioType.x16x9 },
            {"9 x 16", AspectRatioType.x9x16 },
            {"4 x 3", AspectRatioType.x4x3 },
            {"3 x 4", AspectRatioType.x3x4 }
        };

        [SerializeField]
        [Range(0, 3)]
#if UNITY_EDITOR
        [OnValueChanged(nameof(UpdateDimensions))]
#endif
        List<float> dimensionPercentages = new List<float>();

        [SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(UpdateDimensions))]
#endif
        List<float> maxDimensionValues = new List<float>();

#if UNITY_EDITOR
        protected override void OnEnable()
        {
            base.OnEnable();
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

            if (_hasBreakpoints == false) {
                return;
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

        void UpdateDimensions()
        {
            UpdateBreakpointDependencies();
            ExecuteResponsiveAction();
        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();

            if(hasBreakpoints == true) {
                SetValue(breakpointIndex);
            } else {
                SetValue(0);
            }
        }

        void SetValue(int activeIndex)
        {
            double baseDimension = Utils.GetResponsiveWidth(sceneHeight, sceneWidth);

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

            } else if (referenceAspectRatio == AspectRatioType.x4x3) {

                if (ratioType == RatioType.Numerator) {
                    return (referenceVal * 3.0d / 4.0d);
                } else {
                    return (referenceVal * 4.0d / 3.0d);
                }

            } else {

                if (ratioType == RatioType.Numerator) {
                    return (referenceVal* 4.0d / 3.0d);
                } else {
                    return (referenceVal* 3.0d / 4.0d);
                }

            }
        }

    }

}