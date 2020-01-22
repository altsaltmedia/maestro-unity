using System;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ComplexEventManualTrigger : ComplexEventReference
    {
        public void RaiseEvent(GameObject caller)
        {
            GetVariable(caller).StoreCaller(caller);
            GetVariable(caller).Raise();
        }

        public void RaiseEvent(GameObject caller, object value)
        {
            GetVariable(caller).StoreCaller(caller);

            if (value is string) {

                GetVariable(caller).Raise((string)value);

            } else if (value is float) {

                GetVariable(caller).Raise((float)value);

            } else if (value is bool) {

                GetVariable(caller).Raise((bool)value);

            } else if (value is ScriptableObject) {

                GetVariable(caller).Raise((ScriptableObject)value);

            } else {

                GetVariable(caller).Raise(value);
            }
        }

        public void RaiseEvent(GameObject caller, ComplexPayload complexPayload)
        {
            GetVariable(caller).StoreCaller(caller);
            GetVariable(caller).Raise(complexPayload);
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName)
        {
            GetVariable(caller).StoreCaller(caller, sourceScene, sourceName);
            GetVariable(caller).Raise();
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName, ScriptableObject value)
        {
            GetVariable(caller).StoreCaller(caller, sourceScene, sourceName);
            GetVariable(caller).Raise(value);
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName, object value)
        {
            GetVariable(caller).StoreCaller(caller, sourceScene, sourceName);
            GetVariable(caller).Raise(value);
        }
    }
}