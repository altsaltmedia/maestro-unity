using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class ResponsiveAutoUIWidth : ResponsiveRectTransform
    {
        [Required]
        public RectTransform referenceWidth;

        int activeBreakpointIndex = -1;

        [Range(0,1)]
        public List<float> breakpointWidthMargin = new List<float>();

#if UNITY_EDITOR

        float internalWidthValue = 0f;
        float internalWidthMarginValue = 0f;

        protected override void OnRenderObject()
        {
            base.OnRenderObject();

            if (aspectRatioBreakpoints.Count < 1) {
                Debug.LogWarning("ResponsiveAutoUIWidth created with a default breakpoint of 0!", this);
                Utils.ExpandList(aspectRatioBreakpoints, 0);
            }

            if (breakpointWidthMargin.Count <= aspectRatioBreakpoints.Count) {
                Debug.LogWarning("ResponsiveAutoUIWidth, added default breakpoint value(s) of 0.", this);
                Utils.ExpandList(breakpointWidthMargin, aspectRatioBreakpoints.Count);
            }

            DetermineBreakpointIndex();
            if (ValuesChanged() == true) {
                ExecuteResponsiveAction();
            }
        }

        bool ValuesChanged()
        {
            if (Mathf.Approximately(internalWidthMarginValue, breakpointWidthMargin[activeBreakpointIndex]) == false ||
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
            internalWidthMarginValue = breakpointWidthMargin[activeBreakpointIndex];
        }

#endif

        void DetermineBreakpointIndex()
        {
            activeBreakpointIndex = Utils.GetValueIndexInList(aspectRatio.Value, aspectRatioBreakpoints);
        }

        protected override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            DetermineBreakpointIndex();
            float width = referenceWidth.sizeDelta.x;
            float widthModifier = Utils.GetValueFromDesiredPercent(width, breakpointWidthMargin[activeBreakpointIndex]);

            rectTransform.sizeDelta = new Vector2(width - widthModifier, rectTransform.sizeDelta.y);
        }

    }

}