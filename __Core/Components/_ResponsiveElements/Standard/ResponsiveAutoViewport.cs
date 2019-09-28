using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class ResponsiveAutoViewport : ResponsiveCamera
    {
        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        protected FloatReference deviceWidth = new FloatReference();

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        protected FloatReference deviceHeight = new FloatReference();

        [SerializeField]
        public List<Rect> viewportModifiers = new List<Rect>();

        protected override void OnEnable()
        {
            base.OnEnable();
#if UNITY_EDITOR
            UpdateBreakpointDependencies();
#endif
        }

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            SetValue(breakpointIndex);
        }
#if UNITY_EDITOR
        protected override void PopulateDependencies()
        {
            base.PopulateDependencies();

            if (deviceWidth.Variable == null) {
                deviceWidth.Variable = Utils.GetFloatVariable(nameof(VarDependencies.DeviceWidth));
            }

            if (deviceHeight.Variable == null) {
                deviceHeight.Variable = Utils.GetFloatVariable(nameof(VarDependencies.DeviceHeight));
            }
        }

        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();
            if (viewportModifiers.Count == 0) {
                viewportModifiers.Add(new Rect(0,0,0,0));
            }
            if (viewportModifiers.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(viewportModifiers, aspectRatioBreakpoints.Count);
            }
        }
#endif

        public void SetValue(int activeIndex)
        {
#if UNITY_EDITOR
            if (activeIndex >= viewportModifiers.Count) {
                LogBreakpointError();
                return;
            }
#endif
            Rect originalRect = new Rect((deviceWidth.Value - sceneWidth.Value) / 2f, (deviceHeight.Value - sceneHeight.Value) / 2f, sceneWidth.Value, sceneHeight.Value);
            thisCamera.pixelRect = new Rect(originalRect.x + viewportModifiers[activeIndex].x, originalRect.y + viewportModifiers[activeIndex].y, originalRect.width + viewportModifiers[activeIndex].width, originalRect.height + viewportModifiers[activeIndex].height);
        }

    }   
}