using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class SimpleEventTrigger
    {
        [Required]
        [SerializeField]
        SimpleEvent simpleEvent;

        public void RaiseEvent(GameObject caller)
        {
            simpleEvent.StoreCaller(caller);
            simpleEvent.Raise();
        }

        public void RaiseEvent(GameObject caller, string sourceScene, string sourceName)
        {
            simpleEvent.StoreCaller(caller, sourceScene, sourceName);
            simpleEvent.Raise();
        }

        public SimpleEvent GetSimpleEvent()
        {
            return simpleEvent;
        }
    }

}
