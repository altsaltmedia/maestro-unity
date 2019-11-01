using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Sequencing
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
            EventPayload eventPayload = EventPayload.CreateInstance();
            eventPayload.Set(DataType.stringType, this.gameObject.name);
            inputActionComplete.RaiseEvent(this.gameObject, eventPayload);
        }

        private static bool IsPopulated(ComplexEventTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}