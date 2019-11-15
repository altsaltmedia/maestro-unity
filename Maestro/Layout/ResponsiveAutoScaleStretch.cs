using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro.Layout
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
        protected override void OnEnable()
        {
            base.OnEnable();
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
            if(HasBreakpoints == true) {
                SetValue(breakpointIndex);
            } else {
                SetValue(0);
            }
        }

        void SetValue(int activeIndex)
        {
            double width = Utils.GetResponsiveWidth(sceneHeight.Value, sceneWidth.Value);
            float height = Utils.pageHeight * heightMultipliers[activeIndex];

            rectTransform.localScale = new Vector2((float)width * widthMultipliers[activeIndex], height);
        }
    }
}