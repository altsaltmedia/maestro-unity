using System;
using System.Collections.Generic;
using AltSalt.Maestro.Layout;

namespace AltSalt.Maestro.Animation
{
    public class EditClipValues : ParentModuleWindow
    {
        private Dictionary<Type, string> _childWindowData = new Dictionary<Type, string>
        {
            { typeof(EditTMProColorClip), "edit-tm-pro-color-clip" },
            { typeof(EditRectTransformPosClip), "edit-rect-transform-pos-clip" },
            { typeof(EditSpriteColorClip), "edit-sprite-color-clip" },
            { typeof(EditRectTransformScaleClip), "edit-rect-transform-scale-clip" },
            { typeof(EditRectTransformRotationClip), "edit-rect-transform-rotation-clip" },
            { typeof(EditFloatVarClip), "edit-float-var-clip" },
            { typeof(EditColorVarClip), "edit-color-var-clip" },
            { typeof(EditRectTransformResponsivePosClip), "edit-rect-transform-responsive-pos-clip" },
            { typeof(EditRectTransformResponsiveScaleClip), "edit-rect-transform-responsive-scale-clip" }
        };

        protected override Dictionary<Type, string> childWindowData
        {
            get => _childWindowData;
            set => _childWindowData = value;
        }
    }
}