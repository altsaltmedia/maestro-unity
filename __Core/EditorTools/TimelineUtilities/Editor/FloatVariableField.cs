using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[assembly: UxmlNamespacePrefix("AltSalt", "altsalt")]

namespace AltSalt
{
    public class FloatVariableField : ObjectField
    {
        public new class UxmlFactory : UxmlFactory<FloatVariableField, UxmlTraits> { }

        public FloatVariableField() {
            this.objectType = typeof(FloatVariable);
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription bindingPath = new UxmlStringAttributeDescription { name = "binding-path" };
            UxmlStringAttributeDescription label = new UxmlStringAttributeDescription { name = "label" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                ((FloatVariableField)ve).bindingPath = bindingPath.GetValueFromBag(bag, cc);
                ((FloatVariableField)ve).label = label.GetValueFromBag(bag, cc);
            }
        }
    }
}