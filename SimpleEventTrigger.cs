using System;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class SimpleEventTrigger : SimpleEventReference
    {
        public void RaiseEvent(GameObject caller)
        {
            GetVariable(caller).StoreCaller(caller);
            GetVariable(caller).SignalChange();
        }

        public void RaiseEvent(GameObject caller, string sourceName)
        {
            GetVariable(caller).StoreCaller(caller, sourceName);
            GetVariable(caller).SignalChange();
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName)
        {
            GetVariable(caller).StoreCaller(caller, sourceScene, sourceName);
            GetVariable(caller).SignalChange();
        }
    }

}
