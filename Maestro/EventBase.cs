using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{

    public abstract class EventBase : RegisterableScriptableObject, IDependable
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettings _appSettings;

        protected AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
        }

        protected UnityEngine.Object callerObject;
        protected string callerScene = "";
        protected string callerName = "";

        [SerializeField]
        protected bool logCallersOnRaise;

        [SerializeField]
        protected bool logListenersOnRegister;

        [SerializeField]
        protected bool logListenersOnRaise;

        public void StoreCaller(GameObject caller)
        {
            callerObject = caller;
            callerScene = caller.scene.name;
            callerName = caller.name;
        }

        public void StoreCaller(GameObject caller, string sourceScene, string sourceName)
        {
            callerObject = caller;
            callerScene = sourceScene;
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
                Debug.LogError("Caller not registered on " + this.name + ". Are you calling this event directly? Please use an event trigger instead.", this);
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

        abstract protected void LogCaller();

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