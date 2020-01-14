using UnityEngine;

namespace AltSalt.Maestro
{
    public interface ISimpleSignalListener
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