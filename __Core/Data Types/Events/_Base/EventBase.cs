using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{

    public abstract class EventBase : ScriptableObject
    {
    
        [SerializeField]
        [Required]
        protected AppSettings appSettings;

        protected GameObject callerObject;
        protected string callerString = "";

        [SerializeField]
        protected bool logCallersOnRaise;

        [SerializeField]
        protected bool logListenersOnRegister;

        [SerializeField]
        protected bool logListenersOnRaise;

        void Awake()
        {
            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }
        }

        public void StoreCaller(GameObject caller)
        {
            callerObject = caller;
        }

        public void StoreCaller(string caller)
        {
            callerString = caller;
        }

        protected bool CallerRegistered()
        {
            if (callerObject == null && callerString.Length < 1) {
                Debug.LogError("Caller not registered on " + this.name + ". Are you calling this event directly? Please use an event trigger instead.", this);
                return false;
            }

            return true;
        }

        protected void ClearCaller()
        {
            callerObject = null;
            callerString = "";
        }

        abstract protected void LogCaller();

        protected void LogListenersHeading(int listCount)
        {
            if (listCount > 0) {
                Debug.Log("[event] [" + this.name + "] Following listeners activated:", this);
            } else {
                Debug.Log("[event] [" + this.name + "] No listeners registered.", this);
            }
        }

        protected void LogClosingLine()
        {
            Debug.Log("---------");
        }

        public object GetCaller()
        {
            if(callerObject != null) {
                return callerObject;
            }

            if(callerString.Length > 0) {
                return callerString;
            }

            return null;
        }

    }
}
