using System;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing
{
    [Serializable]
    [ExecuteInEditMode]
    public abstract class Input_Module : MonoBehaviour
    {
        protected abstract Input_Controller inputController { get; }
        
        protected UserDataKey userKey => inputController.rootConfig.userKey;
        
        protected InputGroupKey inputGroupKey => inputController.rootConfig.inputGroupKey;

        [SerializeField]
        private int _priority;

        public int priority => _priority;
        
        [SerializeField]
        protected bool _moduleActive = true;

        protected virtual bool moduleActive
        {
            get => _moduleActive;
            set => _moduleActive = value;
        }

        public bool appUtilsRequested => inputController.appUtilsRequested;

        private ComplexEventManualTrigger inputActionComplete =>
            inputController.appSettings.GetInputActionComplete(this.gameObject, inputController.inputGroupKey);
        

        protected virtual void Start()
        {
        }
        
        public void Activate(GameObject callingObject)
        {
            if (callingObject == null) {
                throw new UnassignedReferenceException("You must specify a calling game object.");
            }
            
            if (AppSettings.logEventCallersAndListeners == true) {
                Debug.Log($"{GetType().Name} module  {this.gameObject.name} activated!", this.gameObject);
                Debug.Log($"Status changed by {callingObject.name}", callingObject);
                Debug.Log("--------------------------");
            }
            
            moduleActive = true;
        }
        
        public void Deactivate(GameObject callingObject)
        {
            if (callingObject == null) {
                throw new UnassignedReferenceException("You must specify a calling game object.");
            }
            
            if (AppSettings.logEventCallersAndListeners == true) {
                Debug.Log($"{GetType().Name} module on {this.gameObject.name} deactivated!", this.gameObject);
                Debug.Log($"Status changed by {callingObject.name}", callingObject);
                Debug.Log("--------------------------");
            }
            
            moduleActive = false;
        }

        public void TriggerInputActionComplete()
        {
            // ComplexPayload complexPayload = ComplexPayload.CreateInstance();
            // complexPayload.Set(DataType.stringType, this.gameObject.name);
            // inputActionComplete.RaiseEvent(this.gameObject, complexPayload);

            for (int i = 0; i < inputController.masterSequences.Count; i++) {
                inputController.masterSequences[i].UnlockInputModule(this.gameObject);
            }
        }

        private static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}