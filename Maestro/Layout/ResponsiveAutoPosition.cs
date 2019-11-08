using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro
{
    public class ResponsiveAutoPosition : ResponsiveRectTransform
    {
        [SerializeField]
        List<float> multiplier = new List<float>();

#if UNITY_EDITOR
        protected override void OnEnable()
        {
            base.OnEnable();
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
            double newDimension = Utils.GetResponsiveWidth(sceneHeight.Value, sceneWidth.Value);

            rectTransform.anchoredPosition = new Vector2((float)newDimension * multiplier[activeIndex], rectTransform.anchoredPosition.y);
        }

    }
}