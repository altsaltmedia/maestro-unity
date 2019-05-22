using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class ResponsiveAutoWidthHeight : ResponsiveRectTransform
    {
        [Range(0, 1)]
        [SerializeField]
        List<float> margins = new List<float>();

        [ValueDropdown("dimensionValues")]
        [SerializeField]
        List<DimensionType> orientations = new List<DimensionType>();

        private ValueDropdownList<DimensionType> dimensionValues = new ValueDropdownList<DimensionType>(){
            {"Vertical", DimensionType.Vertical },
            {"Horizontal", DimensionType.Horizontal }
        };

#if UNITY_EDITOR
        float internalWidthMarginValue = 0f;

        protected override void Start()
        {
            base.Start();
            StoreInternalMarginVal();
        }

        void OnEnable()
        {
            UpdateBreakpointDependencies();
        }

        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();
            if (margins.Count == 0) {
                margins.Add(0);
            }
            if (orientations.Count == 0) {
                orientations.Add(DimensionType.Vertical);
            }
            if(margins.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(margins, aspectRatioBreakpoints.Count);
            }
            if(orientations.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(orientations, aspectRatioBreakpoints.Count);
            }
        }

        protected override void OnRenderObject()
        {
            base.OnRenderObject();
            if (MarginChanged() == true) {
                ExecuteResponsiveAction();
            }
        }

        bool MarginChanged()
        {
            GetBreakpointIndex();
            if (Mathf.Approximately(internalWidthMarginValue, margins[breakpointIndex]) == false) {
                StoreInternalMarginVal();
                return true;
            } else {
                return false;
            }
        }

        void StoreInternalMarginVal()
        {
            internalWidthMarginValue = margins[breakpointIndex];
        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            SetValue(breakpointIndex);
        }

        void SetValue(int activeIndex)
        {
#if UNITY_EDITOR
            if (activeIndex >= margins.Count || activeIndex >= orientations.Count) {
                LogBreakpointError();
                return;
            }
#endif

            // Custom equation of an exponential function - equation is in the form y = a^x * b
            // It is derived by taking two (X,Y) coordinates along the line, creating two equations
            // in the form above, then dividing one equation by the other to solve for a and b.
            double newDimension = (Math.Pow(0.561993755433366d, ((double)screenHeight.Value / (double)screenWidth.Value))) * 10.03014554127636d;

            float dimensionModifier = Utils.GetValueFromDesiredPercent((float)newDimension, margins[activeIndex]);

            if (orientations[activeIndex] == DimensionType.Vertical) {
                rectTransform.sizeDelta = new Vector2((float)newDimension - dimensionModifier, rectTransform.sizeDelta.y);
            } else {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (float)newDimension - dimensionModifier);
            }
        }
    }

}