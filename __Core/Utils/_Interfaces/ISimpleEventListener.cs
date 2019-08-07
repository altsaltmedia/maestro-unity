using UnityEngine;

namespace AltSalt
{
    public interface ISimpleEventListener
    {
        void OnEventRaised();
        UnityEngine.Object ParentObject {
            get;
        }
        string SceneName {
            get;
        }
        void LogName(string callingInfo);
    }
}