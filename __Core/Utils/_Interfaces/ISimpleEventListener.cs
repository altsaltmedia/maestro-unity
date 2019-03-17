using UnityEngine;

namespace AltSalt
{
    public interface ISimpleEventListener
    {
        void OnEventRaised();
        GameObject GetGameObject();
        void LogName(string callingInfo);
    }
}