using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro
{

    public abstract class PersistentDataObject : RegisterableScriptableObject, IDependable
    {
        protected UnityEngine.Object callerObject;
        protected string callerScene = "";
        protected string callerName = "";

        [SerializeField]
        protected bool logCallersOnRaise;

        [SerializeField]
        protected bool logListenersOnRegister;

        [SerializeField]
        protected bool logListenersOnRaise;

#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("$"+nameof(title))]
        [FormerlySerializedAs("DeveloperDescription")]
        private string _description = "";
#endif

        protected abstract string title { get; }
        
        public void StoreCaller(GameObject caller)
        {
            callerObject = caller;
            callerScene = caller.scene.name;
            callerName = caller.name;
        }

        public void StoreCaller(GameObject caller, string sourceName)
        {
            callerObject = caller;
            callerScene = caller.scene.name;
            callerName = sourceName;
        }

        public void StoreCaller(UnityEngine.Object caller, string sourceScene, string sourceName)
        {
            callerObject = caller;
            callerScene = sourceScene;
            callerName = sourceName;
        }

        protected bool CallerRegistered()
        {
            if (callerObject == null && callerName.Length < 1) {
                Debug.LogError("Caller not registered on " + this.name + ". Are you accessing this object directly? Please use an event trigger or variable reference instead.", this);
                return false;
            }

            return true;
        }

        protected void ClearCaller()
        {
            callerObject = null;
            callerScene = "";
            callerName = "";
        }

        protected abstract void LogCaller();

        protected void LogListenersHeading(int listCount)
        {
            if (listCount > 0) {
                Debug.Log(string.Format("[event] [{0}] [{1}] Following listeners activated:", callerScene, this.name), this);
            } else {
                Debug.Log(string.Format("[event] [{0}] [{1}] No listeners registered.", callerScene, this.name), this);
            }
        }

        protected void LogClosingLine()
        {
            Debug.Log("[event] ---------");
        }

        public object GetCaller()
        {
            if (callerObject != null) {
                return callerObject;
            }

            if (callerName.Length > 0) {
                return callerName;
            }

            return null;
        }

    }
}