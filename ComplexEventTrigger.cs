using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ComplexEventTrigger : ReferenceBase
    {
        [Required]
        [SerializeField]
        [FormerlySerializedAs("complexEvent")]
        [OnValueChanged(nameof(UpdateReferenceName))]
        private ComplexEvent _complexEvent;

        public ComplexEvent complexEvent {
            get
            {
                if (searchAttempted == false && _complexEvent == null && string.IsNullOrEmpty(referenceName) == false) {
                    searchAttempted = true;
                    LogMissingReferenceMessage(GetType().Name);
                    _complexEvent = Utils.GetScriptableObject(referenceName) as ComplexEvent;
                    if (_complexEvent != null) {
                        LogFoundReferenceMessage(GetType().Name, _complexEvent);
                    }
                }
                return _complexEvent;
            }
            set => _complexEvent = value;
        }

        protected override void UpdateReferenceName()
        {
            if (complexEvent != null) {
                referenceName = complexEvent.name;
            }
        }

        public void RaiseEvent(GameObject caller)
        {
            complexEvent.StoreCaller(caller);
            complexEvent.Raise();
        }

        public void RaiseEvent(GameObject caller, object value)
        {
            complexEvent.StoreCaller(caller);

            if (value is string) {

                complexEvent.Raise((string)value);

            } else if (value is float) {

                complexEvent.Raise((float)value);

            } else if (value is bool) {

                complexEvent.Raise((bool)value);

            } else if (value is ScriptableObject) {

                complexEvent.Raise((ScriptableObject)value);

            } else {

                complexEvent.Raise(value);
            }
        }

        public void RaiseEvent(GameObject caller, ComplexPayload complexPayload)
        {
            complexEvent.StoreCaller(caller);
            complexEvent.Raise(complexPayload);
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName)
        {
            complexEvent.StoreCaller(caller, sourceScene, sourceName);
            complexEvent.Raise();
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName, ScriptableObject value)
        {
            complexEvent.StoreCaller(caller, sourceScene, sourceName);
            complexEvent.Raise(value);
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName, object value)
        {
            complexEvent.StoreCaller(caller, sourceScene, sourceName);
            complexEvent.Raise(value);
        }
    }
}