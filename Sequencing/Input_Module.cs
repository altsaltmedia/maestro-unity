using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Sequencing
{
    public abstract class Input_Module : MonoBehaviour
    {
        [SerializeField]
        private int _priority;

        public int priority
        {
            get => _priority;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventTrigger _inputActionComplete;

        private ComplexEventTrigger inputActionComplete
        {
            get => _inputActionComplete;
            set => _inputActionComplete = value;
        }

        public void TriggerInputActionComplete()
        {
            ComplexPayload complexPayload = ComplexPayload.CreateInstance();
            complexPayload.Set(DataType.stringType, this.gameObject.name);
            inputActionComplete.RaiseEvent(this.gameObject, complexPayload);
        }

        private static bool IsPopulated(ComplexEventTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}