using System;
using System.Collections.Generic;
using AltSalt.Maestro.Layout;

namespace AltSalt.Maestro.Animation
{
    public class EditAnimationClips : ParentModuleWindow
    {
        private Dictionary<Type, string> _childWindowData = new Dictionary<Type, string>
        {
            { typeof(EditTMProColorClip), nameof(EditTMProColorClip) },
            { typeof(EditRectTransformPosClip), nameof(EditRectTransformPosClip) },
            { typeof(EditSpriteColorClip), nameof(EditSpriteColorClip) },
            { typeof(EditRectTransformScaleClip), nameof(EditRectTransformScaleClip) },
            { typeof(EditRectTransformRotationClip), nameof(EditRectTransformRotationClip) },
            { typeof(EditFloatVarClip), nameof(EditFloatVarClip) },
            { typeof(EditColorVarClip), nameof(EditColorVarClip) },
            { typeof(EditRectTransformResponsivePosClip), nameof(EditRectTransformResponsivePosClip) },
            { typeof(EditRectTransformResponsiveScaleClip), nameof(EditRectTransformResponsiveScaleClip) }
        };

        protected override Dictionary<Type, string> childWindowData
        {
            get => _childWindowData;
            set => _childWindowData = value;
        }
    }
}