using System;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ComplexEventManualTrigger : ComplexEventReference
    {
        public void RaiseEvent(GameObject caller)
        {
            (GetVariable() as ComplexEvent).StoreCaller(caller);
            (GetVariable() as ComplexEvent).Raise();
        }

        public void RaiseEvent(GameObject caller, object value)
        {
            (GetVariable() as ComplexEvent).StoreCaller(caller);

            if (value is string) {

                (GetVariable() as ComplexEvent).Raise((string)value);

            } else if (value is float) {

                (GetVariable() as ComplexEvent).Raise((float)value);

            } else if (value is bool) {

                (GetVariable() as ComplexEvent).Raise((bool)value);

            } else if (value is ScriptableObject) {

                (GetVariable() as ComplexEvent).Raise((ScriptableObject)value);

            } else {

                (GetVariable() as ComplexEvent).Raise(value);
            }
        }

        public void RaiseEvent(GameObject caller, ComplexPayload complexPayload)
        {
            (GetVariable() as ComplexEvent).StoreCaller(caller);
            (GetVariable() as ComplexEvent).Raise(complexPayload);
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName)
        {
            (GetVariable() as ComplexEvent).StoreCaller(caller, sourceScene, sourceName);
            (GetVariable() as ComplexEvent).Raise();
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName, ScriptableObject value)
        {
            (GetVariable() as ComplexEvent).StoreCaller(caller, sourceScene, sourceName);
            (GetVariable() as ComplexEvent).Raise(value);
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName, object value)
        {
            (GetVariable() as ComplexEvent).StoreCaller(caller, sourceScene, sourceName);
            (GetVariable() as ComplexEvent).Raise(value);
        }
    }
}