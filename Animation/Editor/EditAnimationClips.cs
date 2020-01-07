using System;
using System.Collections.Generic;

namespace AltSalt.Maestro.Animation
{
    public class EditAnimationClips : ParentModuleWindow
    {
        private Dictionary<Type, string> _childWindowData = new Dictionary<Type, string>
        {
            { typeof(EditColorClip), nameof(EditColorClip) },
            { typeof(EditResponsiveVector3Clip), nameof(EditResponsiveVector3Clip) },
            { typeof(EditFloatClip), nameof(EditFloatClip) },
        };
        
        protected override Dictionary<Type, string> childWindowData
        {
            get => _childWindowData;
            set => _childWindowData = value;
        }
    }
}