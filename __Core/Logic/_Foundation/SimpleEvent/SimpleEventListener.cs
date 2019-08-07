using System;
using UnityEngine;
using UnityEngine.Events;

namespace AltSalt
{
    [ExecuteInEditMode]
    public class SimpleEventListener : ISimpleEventListener
    {
        public delegate void OnTargetEventDelegate();
        public event OnTargetEventDelegate OnTargetEventExecuted = () => { };

        readonly SimpleEvent targetEvent;
        readonly GameObject parentGameObject;
        readonly UnityEngine.Object parentObject;

        public UnityEngine.Object ParentObject
        {
            get {
                if(parentGameObject != null) {
                    return parentGameObject;
                } else {
                    return parentObject;
                }
            }
        }

        string sceneName;
        public string SceneName {
            get {
                return sceneName;
            }
        }

        public SimpleEventListener(SimpleEvent eventToRegister, GameObject parentObject)
        {
            this.targetEvent = eventToRegister;
            this.parentGameObject = parentObject;
            this.sceneName = parentObject.scene.name;
            this.targetEvent.RegisterListener(this);
        }

        public SimpleEventListener(SimpleEvent eventToRegister, UnityEngine.Object parentObject, string sceneName)
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
            Debug.Log(callingInfo + ParentObject, ParentObject);
        }
    }
}