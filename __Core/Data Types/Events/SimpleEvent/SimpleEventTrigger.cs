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

        public static void RaiseEvent(SimpleEvent targetEvent, GameObject caller)
        {
            targetEvent.StoreCaller(caller);
            targetEvent.Raise();
        }

        public void RaiseEvent(GameObject caller)
        {
            simpleEvent.StoreCaller(caller);
            simpleEvent.Raise();
        }

        public SimpleEvent GetSimpleEvent()
        {
            return simpleEvent;
        }
    }

}
