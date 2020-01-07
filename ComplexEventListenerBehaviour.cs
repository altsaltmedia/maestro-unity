/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [System.Serializable]
    public class ComplexUnityEventHandler : UnityEvent<EventPayload> { }

    [ExecuteInEditMode]
    public class ComplexEventListenerBehaviour : MonoBehaviour, ISkipRegistration
	{
		[Required]
		public ComplexEvent Event;

        #if UNITY_EDITOR
            [Multiline]
            public string DeveloperDescription = "";
        #endif
		
		[ValidateInput("IsPopulated")]
		public ComplexUnityEventHandler Response;

        [SerializeField]
        [InfoBox("Specifies whether this dependency should be recorded when the RegisterDependencies tool is used.")]
        bool doNotRecord;

        public bool DoNotRecord {
            get {
                return doNotRecord;
            }
        }

        // Use this for initialization
        private void OnEnable()
		{
			Event.RegisterListener(this);
		}
		
		// Update is called once per frame
		private void OnDisable()
		{
			Event.UnregisterListener(this);
		}

        public void OnEventRaised(EventPayload eventPayload)
		{
            Response.Invoke(eventPayload);
		}
		
		private static bool IsPopulated(ComplexUnityEventHandler attribute)
		{
			return Utils.IsPopulated(attribute);
		}
	}
	   
}