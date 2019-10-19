using UnityEngine;

namespace AltSalt.Sequencing
{
    public class InputModule : MonoBehaviour
    {
        [SerializeField]
        private int _priority;

        public int priority
        {
            get => _priority;
        }

        [SerializeField]
        private ComplexEventTrigger _inputActionComplete;

        public ComplexEventTrigger inputActionComplete
        {
            get => _inputActionComplete;
            set => _inputActionComplete = value;
        }

        protected void TriggerInputActionComplete()
        {
            EventPayload eventPayload = EventPayload.CreateInstance();
            eventPayload.Set(DataType.stringType, this.gameObject.name);
            inputActionComplete.RaiseEvent(this.gameObject, eventPayload);
        }
    }
}