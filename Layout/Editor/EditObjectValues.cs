using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Layout
{
    public class EditObjectValues : ParentModuleWindow
    {
        private Dictionary<Type, string> _childWindowData = new Dictionary<Type, string>
        {
            {typeof(EditRectTransformComponent), nameof(EditRectTransformComponent)},
            {typeof(EditTMProComponent), nameof(EditTMProComponent)},
            {typeof(EditSpriteComponent), nameof(EditSpriteComponent)},
            {typeof(EditScrollSnapController), nameof(EditScrollSnapController)}
        };

        protected override Dictionary<Type, string> childWindowData
        {
            get => _childWindowData;
            set => _childWindowData = value;
        }

    }
}