using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class ResponsiveAutoUIWidth : ResponsiveRectTransform
    {
        [Required]
        public RectTransform referenceWidth;

        [Range(0,1)]
        public List<float> breakpointWidthMargin = new List<float>();

#if UNITY_EDITOR

        float internalWidthValue = 0f;
        float internalWidthMarginValue = 0f;

        protected override void OnRenderObject()
        {
            base.OnRenderObject();

            if (_aspectRatioBreakpoints.Count < 1) {
                Debug.LogWarning("ResponsiveAutoUIWidth created with a default breakpoint of 0!", this);
                Utils.ExpandList(_aspectRatioBreakpoints, 0);
            }

            if (breakpointWidthMargin.Count <= _aspectRatioBreakpoints.Count) {
                Debug.LogWarning("ResponsiveAutoUIWidth, added default breakpoint value(s) of 0.", this);
                Utils.ExpandList(breakpointWidthMargin, _aspectRatioBreakpoints.Count);
            }

            if (ValuesChanged() == true) {
                ExecuteResponsiveAction();
            }
        }

        bool ValuesChanged()
        {
            if (Mathf.Approximately(internalWidthMarginValue, breakpointWidthMargin[breakpointIndex]) == false ||
                Mathf.Approximately(internalWidthValue, referenceWidth.sizeDelta.x) == false) {
                StoreInternalValues();
                return true;
            }
            else {
                return false;
            }
        }

        void StoreInternalValues()
        {
            internalWidthValue = referenceWidth.sizeDelta.x;
            internalWidthMarginValue = breakpointWidthMargin[breakpointIndex];
        }

#endif
        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            SetValue(breakpointIndex);
        }

        void SetValue(int activeIndex)
        {
            float width = referenceWidth.sizeDelta.x;
            float widthModifier = Utils.GetValueFromDesiredPercent(width, breakpointWidthMargin[activeIndex]);

            rectTransform.sizeDelta = new Vector2(width - widthModifier, rectTransform.sizeDelta.y);
        }

    }

}