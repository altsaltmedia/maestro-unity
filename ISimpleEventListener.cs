using UnityEngine;

namespace AltSalt.Maestro
{
    public interface ISimpleEventListener
    {
        void OnEventRaised();
        
        UnityEngine.Object parentObject {
            get;
        }
        string sceneName {
            get;
        }
        void LogName(string callingInfo);
    }
}