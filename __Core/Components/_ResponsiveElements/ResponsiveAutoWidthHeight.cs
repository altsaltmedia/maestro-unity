using System;
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
        public float margin = 0f;

        [ValueDropdown("dimensionValues")]
        [SerializeField]
        DimensionType orientation;

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

        protected override void OnRenderObject()
        {
            base.OnRenderObject();
            if (MarginChanged() == true) {
                ExecuteResponsiveAction();
            }
        }

        bool MarginChanged()
        {
            if (Mathf.Approximately(internalWidthMarginValue, margin) == false) {
                StoreInternalMarginVal();
                return true;
            }
            else {
                return false;
            }
        }

        void StoreInternalMarginVal()
        {
            internalWidthMarginValue = margin;
        }

#endif

        protected override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            // Custom equation of an exponential function - equation is in the form y = a^x * b
            // It is derived by taking two (X,Y) coordinates along the line, creating two equations
            // in the form above, then dividing one equation by the other to solve for a and b.
            double newDimension = 0d;

            newDimension = (Math.Pow(0.561993755433366d, ((double)screenHeight.Value / (double)screenWidth.Value))) * 10.03014554127636d;
            float dimensionModifier = Utils.GetValueFromDesiredPercent((float)newDimension, margin);

            if (orientation == DimensionType.Vertical) {
                rectTransform.sizeDelta = new Vector2((float)newDimension - dimensionModifier, rectTransform.sizeDelta.y);
            } else {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (float)newDimension - dimensionModifier);
            }

            Debug.Log(rectTransform.sizeDelta.x.ToString("F4")+"/"+rectTransform.sizeDelta.y.ToString("F4"));
        }
    }

}