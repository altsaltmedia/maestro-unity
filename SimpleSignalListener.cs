using UnityEngine;

namespace AltSalt.Maestro
{
    [ExecuteInEditMode]
    public class SimpleSignalListener : ISimpleSignalListener
    {
        public delegate void OnTargetEventDelegate();
        public event OnTargetEventDelegate OnTargetEventExecuted = () => { };

        private SimpleSignal _targetEvent;

        private SimpleSignal targetEvent
        {
            get => _targetEvent;
            set => _targetEvent = value;
        }

        private GameObject _parentGameObject;

        private GameObject parentGameObject
        {
            get => _parentGameObject;
            set => _parentGameObject = value;
        }
        
        private UnityEngine.Object _parentObject;
        
        public UnityEngine.Object parentObject
        {
            get
            {
                if(parentGameObject != null) {
                    return parentGameObject;
                }

                return parentObject;
            }
            private set => _parentObject = value;
        }

        private string _sceneName;
        
        public string sceneName
        {
            get => _sceneName;
            set => _sceneName = value;
        }

        public SimpleSignalListener(SimpleEvent eventToRegister, GameObject parentObject)
        {
            this.targetEvent = eventToRegister;
            this.parentGameObject = parentObject;
            this.sceneName = parentObject.scene.name;
            this.targetEvent.RegisterListener(this);
        }

        public SimpleSignalListener(SimpleEvent eventToRegister, UnityEngine.Object parentObject, string sceneName)
        {
            this.targetEvent = eventToRegister;
            this.parentObject = parentObject;
            this.sceneName = sceneName;
            this.targetEvent.RegisterListener(this);
        }

        public void OnEventRaised()
        {
            OnTargetEventExecuted();
        }

        public void DestroyListener()
        {
            targetEvent.UnregisterListener(this);
        }

        public void LogName(string callingInfo)
        {
            Debug.Log(callingInfo + parentObject, parentObject);
        }
    }
}