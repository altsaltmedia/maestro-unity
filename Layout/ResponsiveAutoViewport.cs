using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
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

            if (deviceWidth.GetVariable(this.gameObject) == null) {
                deviceWidth.SetVariable(Utils.GetFloatVariable(nameof(VarDependencies.DeviceWidth)));
            }

            if (deviceHeight.GetVariable(this.gameObject) == null) {
                deviceHeight.SetVariable(Utils.GetFloatVariable(nameof(VarDependencies.DeviceHeight)));
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
            Rect originalRect = new Rect((deviceWidth.GetValue(this.gameObject) - sceneWidth) / 2f, (deviceHeight.GetValue(this.gameObject) - sceneHeight) / 2f, sceneWidth, sceneHeight);
            thisCamera.pixelRect = new Rect(originalRect.x, originalRect.y, originalRect.width, originalRect.height);
            thisCamera.rect = new Rect(thisCamera.rect.x + viewportModifiers[activeIndex].x,
                thisCamera.rect.y + +viewportModifiers[activeIndex].y,
                thisCamera.rect.width + viewportModifiers[activeIndex].width,
                thisCamera.rect.height + viewportModifiers[activeIndex].height);
        }

    }   
}