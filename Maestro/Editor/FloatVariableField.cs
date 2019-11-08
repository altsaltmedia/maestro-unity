using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[assembly: UxmlNamespacePrefix("AltSalt", "altsalt")]

namespace AltSalt.Maestro
{
    public class FloatVariableField : ObjectField
    {
        public new class UxmlFactory : UxmlFactory<FloatVariableField, UxmlTraits> { }

        public FloatVariableField() {
            this.objectType = typeof(FloatVariable);
            FloatVariableChangeEvent.RegisterFloatEventType();
            SynthesizeFloatVarChangeEvent();
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription bindingPath = new UxmlStringAttributeDescription { name = "binding-path" };
            UxmlStringAttributeDescription label = new UxmlStringAttributeDescription { name = "label" };
            UxmlStringAttributeDescription name = new UxmlStringAttributeDescription { name = "name" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                ((FloatVariableField)ve).bindingPath = bindingPath.GetValueFromBag(bag, cc);
                ((FloatVariableField)ve).label = label.GetValueFromBag(bag, cc);
                ((FloatVariableField)ve).name = name.GetValueFromBag(bag, cc);
            }
        }

        void SynthesizeFloatVarChangeEvent()
        {
            using (ChangeEvent<FloatVariable> changeEvent = ChangeEvent<FloatVariable>.GetPooled()) {
                SendEvent(changeEvent);
            }
        }

        public class FloatVariableChangeEvent : ChangeEvent<FloatVariable>
        {
            public static void RegisterFloatEventType() {
                RegisterEventType();
            }
        }
    }
}