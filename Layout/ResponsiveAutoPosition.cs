using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveAutoPosition : ResponsiveRectTransform
    {
        [SerializeField]
        [InfoBox("Automatically sets the X position as a percentage of the container's width, i.e. a value of 0 " +
                 "will set the X position to 0, wheras .5 will set X position to half the container's width.")]
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
                multiplier.Add(0f);
            }
        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            SetValue(breakpointIndex);
        }

        private void SetValue(int activeIndex)
        {
            double newDimension = Utils.GetResponsiveWidth(sceneHeight, sceneWidth);

            rectTransform.anchoredPosition = new Vector2((float)newDimension * multiplier[activeIndex], rectTransform.anchoredPosition.y);
        }

    }
}