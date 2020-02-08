using System;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class SimpleEventTrigger : SimpleEventReference, IPersistentEventTrigger
    {
        public void RaiseEvent(GameObject caller)
        {
            (GetVariable() as SimpleEvent).StoreCaller(caller);
            (GetVariable() as SimpleEvent).SignalChange();
        }

        public void RaiseEvent(GameObject caller, string sourceName)
        {
            (GetVariable() as SimpleEvent).StoreCaller(caller, sourceName);
            (GetVariable() as SimpleEvent).SignalChange();
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName)
        {
            (GetVariable() as SimpleEvent).StoreCaller(caller, sourceScene, sourceName);
            (GetVariable() as SimpleEvent).SignalChange();
        }
    }

}
