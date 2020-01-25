using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Sequencing
{
    [Serializable]
    [ExecuteInEditMode]
    public abstract class Input_Module : MonoBehaviour
    {
        protected abstract Input_Controller inputController { get; }

            [SerializeField]
        private int _priority;

        public int priority => _priority;

        private ComplexEventManualTrigger inputActionComplete =>
            inputController.appSettings.GetInputActionComplete(this.gameObject, inputController.inputGroupKey);
        
        public void TriggerInputActionComplete()
        {
            ComplexPayload complexPayload = ComplexPayload.CreateInstance();
            complexPayload.Set(DataType.stringType, this.gameObject.name);
            inputActionComplete.RaiseEvent(this.gameObject, complexPayload);
        }

        private static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}