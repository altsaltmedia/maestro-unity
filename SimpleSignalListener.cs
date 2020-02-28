using UnityEngine;
using System.Linq;

namespace AltSalt.Maestro
{
    [ExecuteInEditMode]
    public class SimpleSignalListener : ISimpleSignalListener
    {
        public delegate void TargetSignalExecutedHandler();
        
        private event TargetSignalExecutedHandler _targetEventExecuted = () => { };
        
        public event TargetSignalExecutedHandler targetEventExecuted
        {
            add
            {
                if (_targetEventExecuted == null
                    || _targetEventExecuted.GetInvocationList().Contains(value) == false) {
                    _targetEventExecuted += value;
                }
            }
            remove => _targetEventExecuted -= value;
        }

        private SimpleSignal _targetSignal;

        private SimpleSignal targetSignal
        {
            get => _targetSignal;
            set => _targetSignal = value;
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
            private set => _sceneName = value;
        }

        public SimpleSignalListener(SimpleSignal signalToRegister, GameObject parentObject)
        {
            this.targetSignal = signalToRegister;
            this.parentGameObject = parentObject;
            this.sceneName = parentObject.scene.name;
            this.targetSignal.RegisterListener(this);
        }

        public SimpleSignalListener(SimpleSignal signalToRegister, UnityEngine.Object parentObject, string sceneName)
        {
            this.targetSignal = signalToRegister;
            this.parentObject = parentObject;
            this.sceneName = sceneName;
            this.targetSignal.RegisterListener(this);
        }

        public void OnEventRaised()
        {
            _targetEventExecuted.Invoke();
        }

        public void DestroyListener()
        {
            targetSignal.UnregisterListener(this);
        }

        public void LogName(string callingInfo)
        {
            Debug.Log(callingInfo + parentObject, parentObject);
        }
    }
}