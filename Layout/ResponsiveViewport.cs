using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveViewport : ResponsiveCamera, IResponsiveSaveable
    {

        [SerializeField]
        public List<Rect> viewportValues = new List<Rect>();

#if UNITY_EDITOR

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateBreakpointDependencies();
        }
        
        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();
            if (viewportValues.Count == 0) {
                viewportValues.Add(thisCamera.rect);
            }
            
            if (viewportValues.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(viewportValues, aspectRatioBreakpoints.Count);
            }
        }
        
        [InfoBox("Saves the viewport value at the current breakpoint index.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SaveValue()
        {
            if (aspectRatioBreakpoints.Count < 1) {
                Debug.LogWarning("You must specify at least one breakpoint to save element values.", this);
                return;
            }

            int targetBreakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio, aspectRatioBreakpoints);

            Utils.ExpandList(viewportValues, targetBreakpointIndex);
            viewportValues[targetBreakpointIndex] = thisCamera.rect;
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

        public void SetValue(int activeIndex)
        {
#if UNITY_EDITOR
            if (activeIndex >= viewportValues.Count) {
                LogBreakpointError();
                return;
            }
#endif
            thisCamera.rect = viewportValues[activeIndex];
        }

    }
}